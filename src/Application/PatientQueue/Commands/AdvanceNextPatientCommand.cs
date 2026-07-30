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

namespace Application.PatientQueue.Commands;

public record NextPatientResponse(
    PatientQueueTicketResponse? CompletedPatient,
    PatientQueueTicketResponse? ActivePatient,
    int RemainingQueueCount,
    bool HasNextPatient);

public record AdvanceNextPatientCommand(
    Guid DoctorId,
    Guid? PracticeCentreId = null,
    DateTime? VisitDate = null) : ICommand<NextPatientResponse>;

internal sealed class AdvanceNextPatientCommandHandler(IApplicationDbContext dbContext)
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

        List<PatientQueueTicket> ticketsForDay = new();

        if (request.DoctorId != Guid.Empty)
        {
            ticketsForDay = await query
                .Where(q => q.DoctorId == request.DoctorId && q.VisitDate == targetDate)
                .ToListAsync(cancellationToken);
        }

        // Fallback 1: Query any doctor's tickets for targetDate if no match for specified DoctorId
        if (!ticketsForDay.Any())
        {
            ticketsForDay = await query
                .Where(q => q.VisitDate == targetDate)
                .ToListAsync(cancellationToken);
        }

        // Fallback 2: Query active or waiting tickets overall if no tickets found for today
        if (!ticketsForDay.Any())
        {
            ticketsForDay = await query
                .Where(q => q.Status == PatientQueueStatus.InConsultation 
                         || q.Status == PatientQueueStatus.Waiting 
                         || q.Status == PatientQueueStatus.Ready)
                .ToListAsync(cancellationToken);
        }

        // 1. Finalize current active patient in consultation
        var activeTicket = ticketsForDay.FirstOrDefault(t => t.Status == PatientQueueStatus.InConsultation);
        if (activeTicket != null)
        {
            activeTicket.Status = PatientQueueStatus.Completed;
            activeTicket.CompletedAt = DateTime.UtcNow;
        }

        // 2. Advance next queued patient (Waiting or Ready state)
        var nextTicket = ticketsForDay
            .Where(t => t.Status == PatientQueueStatus.Waiting || t.Status == PatientQueueStatus.Ready)
            .OrderBy(t => t.QueueOrder)
            .ThenBy(t => t.CreatedAt)
            .FirstOrDefault();

        if (nextTicket != null)
        {
            nextTicket.Status = PatientQueueStatus.InConsultation;
            nextTicket.CalledAt = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        // Count remaining waiting/ready tickets for the day
        int remainingCount = ticketsForDay.Count(t => t.Status == PatientQueueStatus.Waiting || t.Status == PatientQueueStatus.Ready);

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
