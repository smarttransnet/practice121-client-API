using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.PatientQueue.Queries;
using Domain.PatientQueue;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

using Application.Abstractions.Realtime;

namespace Application.PatientQueue.Commands;

public record NextPatientResponse(
    PatientQueueTicketResponse? CompletedPatient,
    PatientQueueTicketResponse? ActivePatient,
    int RemainingQueueCount,
    bool HasNextPatient);

public record AdvanceNextPatientCommand(
    Guid DoctorId,
    Guid? PracticeCentreId = null,
    DateTime? VisitDate = null,
    Guid? SessionId = null) : ICommand<NextPatientResponse>;

internal sealed class AdvanceNextPatientCommandHandler(
    IApplicationDbContext dbContext,
    IPatientQueueNotifier? notifier = null)
    : ICommandHandler<AdvanceNextPatientCommand, NextPatientResponse>
{
    public async Task<Result<NextPatientResponse>> Handle(AdvanceNextPatientCommand request, CancellationToken cancellationToken)
    {
        var targetDate = request.VisitDate?.Date ?? DateTime.UtcNow.Date;

        var query = dbContext.PatientQueueTickets.AsQueryable();

        if (request.PracticeCentreId.HasValue && request.PracticeCentreId.Value != Guid.Empty)
        {
            query = query.Where(q => q.PracticeCentreId == request.PracticeCentreId.Value);
        }

        if (request.SessionId.HasValue && request.SessionId.Value != Guid.Empty)
        {
            query = query.Where(q => q.SessionId == request.SessionId.Value);
        }

        List<PatientQueueTicket> ticketsForDay = new();

        if (request.DoctorId != Guid.Empty)
        {
            ticketsForDay = await query
                .Where(q => q.DoctorId == request.DoctorId && q.VisitDate == targetDate)
                .ToListAsync(cancellationToken);
        }

        // Overtime / Extended Session Fallback: If session exceeds scheduled timestamp or crosses date boundary,
        // include any active (InConsultation) or Ready ticket for this doctor and practice centre.
        if (!ticketsForDay.Any(t => t.Status == PatientQueueStatus.Ready || t.Status == PatientQueueStatus.InConsultation))
        {
            var overtimeTickets = await query
                .Where(q => (request.DoctorId == Guid.Empty || q.DoctorId == request.DoctorId)
                         && (q.Status == PatientQueueStatus.InConsultation || q.Status == PatientQueueStatus.Ready))
                .ToListAsync(cancellationToken);

            if (overtimeTickets.Any())
            {
                foreach (var ot in overtimeTickets)
                {
                    if (!ticketsForDay.Any(existing => existing.Id == ot.Id))
                    {
                        ticketsForDay.Add(ot);
                    }
                }
            }
        }

        // 1. Finalize current active patient in consultation
        var activeTicket = ticketsForDay.FirstOrDefault(t => t.Status == PatientQueueStatus.InConsultation);
        if (activeTicket != null)
        {
            activeTicket.Status = PatientQueueStatus.Completed;
            activeTicket.CompletedAt = DateTime.UtcNow;
        }

        // 2. Advance next queued patient (Prioritize Ready, then Waiting)
        var nextTicket = ticketsForDay
            .Where(t => t.Status == PatientQueueStatus.Ready || t.Status == PatientQueueStatus.Waiting)
            .OrderByDescending(t => t.Status == PatientQueueStatus.Ready)
            .ThenBy(t => t.QueueOrder)
            .ThenBy(t => t.CreatedAt)
            .FirstOrDefault();

        if (nextTicket != null)
        {
            if (!nextTicket.PatientId.HasValue)
            {
                return Result.Failure<NextPatientResponse>(Error.Validation(
                    "PatientQueueTicket.PatientIdRequired",
                    "The next patient in queue is not linked to a patientId. Link or register the patient before consultation."));
            }

            nextTicket.Status = PatientQueueStatus.InConsultation;
            nextTicket.CalledAt = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        if (notifier != null && request.PracticeCentreId.HasValue)
        {
            await notifier.NotifyQueueUpdatedAsync(request.PracticeCentreId.Value, request.DoctorId, targetDate, cancellationToken);
        }

        // Count remaining ready tickets for the day
        int remainingCount = ticketsForDay.Count(t => t.Status == PatientQueueStatus.Ready);

        // Hydrate Patient Names
        var relevantTickets = new List<PatientQueueTicket>();
        if (activeTicket != null)
        {
            relevantTickets.Add(activeTicket);
        }
        if (nextTicket != null)
        {
            relevantTickets.Add(nextTicket);
        }

        var patientById = new Dictionary<Guid, string>();
        var patientByMobile = new Dictionary<string, string>();

        if (relevantTickets.Any())
        {
            var patientIds = relevantTickets.Where(t => t.PatientId.HasValue).Select(t => t.PatientId!.Value).Distinct().ToList();
            var patientMobiles = relevantTickets.Select(t => t.PatientMobile).Distinct().ToList();

            var patients = await dbContext.PatientAccounts.AsNoTracking()
                .Where(p => patientIds.Contains(p.Id) || patientMobiles.Contains(p.MobileNumber))
                .ToListAsync(cancellationToken);

            patientById = patients.ToDictionary(p => p.Id, p => $"{p.FirstName} {p.LastName}".Trim());
            patientByMobile = patients
                .GroupBy(p => p.MobileNumber)
                .ToDictionary(g => g.Key, g => $"{g.First().FirstName} {g.First().LastName}".Trim());
        }

        PatientQueueTicketResponse? MapResponse(PatientQueueTicket? t)
        {
            if (t == null)
            {
                return null;
            }
            string name = "Unknown Patient";
            if (t.PatientId.HasValue && patientById.TryGetValue(t.PatientId.Value, out var nameById))
            {
                name = nameById;
            }
            else if (patientByMobile.TryGetValue(t.PatientMobile, out var nameByMobile))
            {
                name = nameByMobile;
            }

            return new PatientQueueTicketResponse(
                t.Id,
                t.QueueNumber,
                t.QueueOrder,
                t.PatientMobile,
                name,
                t.DoctorId,
                t.PracticeCentreId,
                t.VisitDate,
                t.Status,
                t.Priority,
                t.CreatedAt,
                t.CalledAt,
                t.CompletedAt,
                t.SessionId,
                t.PatientId);
        }

        var response = new NextPatientResponse(
            CompletedPatient: MapResponse(activeTicket),
            ActivePatient: MapResponse(nextTicket),
            RemainingQueueCount: remainingCount,
            HasNextPatient: nextTicket != null);

        return response;
    }
}
