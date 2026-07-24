using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.PatientQueue;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.PatientQueue.Queries;

public record PatientQueueTicketResponse(
    Guid Id,
    int QueueNumber,
    int QueueOrder,
    string PatientMobile,
    string PatientName,
    Guid DoctorId,
    Guid PracticeCentreId,
    DateTime VisitDate,
    PatientQueueStatus Status,
    PatientQueuePriority Priority,
    DateTime CreatedAt,
    DateTime? CalledAt,
    DateTime? CompletedAt);

public record GetPatientQueueListQuery(
    Guid PracticeCentreId,
    Guid? DoctorId,
    DateTime? VisitDate) : IQuery<List<PatientQueueTicketResponse>>;

internal sealed class GetPatientQueueListQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetPatientQueueListQuery, List<PatientQueueTicketResponse>>
{
    public async Task<Result<List<PatientQueueTicketResponse>>> Handle(GetPatientQueueListQuery request, CancellationToken cancellationToken)
    {
        var targetDate = request.VisitDate?.Date ?? DateTime.UtcNow.Date;

        var query = dbContext.PatientQueueTickets.AsNoTracking()
            .Where(q => q.PracticeCentreId == request.PracticeCentreId && q.VisitDate == targetDate);

        if (request.DoctorId.HasValue && request.DoctorId.Value != Guid.Empty)
        {
            query = query.Where(q => q.DoctorId == request.DoctorId.Value);
        }

        var tickets = await query
            .OrderBy(q => q.QueueOrder)
            .ToListAsync(cancellationToken);

        var patientIds = tickets.Where(t => t.PatientId.HasValue).Select(t => t.PatientId!.Value).Distinct().ToList();
        var patientMobiles = tickets.Select(t => t.PatientMobile).Distinct().ToList();

        var patients = await dbContext.PatientAccounts.AsNoTracking()
            .Where(p => patientIds.Contains(p.Id) || patientMobiles.Contains(p.MobileNumber))
            .ToListAsync(cancellationToken);

        var patientById = patients.ToDictionary(p => p.Id, p => $"{p.FirstName} {p.LastName}".Trim());
        var patientByMobile = patients
            .GroupBy(p => p.MobileNumber)
            .ToDictionary(g => g.Key, g => $"{g.First().FirstName} {g.First().LastName}".Trim());

        var response = tickets.Select(t =>
        {
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
                t.CompletedAt);
        }).ToList();

        return response;
    }
}
