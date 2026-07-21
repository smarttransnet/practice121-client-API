using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Appointments.Queries;

public record GetCentreAvailabilityQuery(
    Guid DoctorAccountId,
    Guid PracticeCentreId,
    DateOnly From,
    DateOnly To) : IQuery<List<DayAvailabilityResponse>>;

public record DayAvailabilityResponse(
    DateOnly Date,
    int? TotalSlots,
    int BookedCount,
    bool IsFull);

internal sealed class GetCentreAvailabilityQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetCentreAvailabilityQuery, List<DayAvailabilityResponse>>
{
    public async Task<Result<List<DayAvailabilityResponse>>> Handle(
        GetCentreAvailabilityQuery request,
        CancellationToken cancellationToken)
    {
        // Load practice centre with session groups
        var practiceCentre = await dbContext.PracticeCentres
            .AsNoTracking()
            .Include(pc => pc.SessionGroups)
            .FirstOrDefaultAsync(
                pc => pc.Id == request.PracticeCentreId && pc.DoctorId == request.DoctorAccountId,
                cancellationToken);

        if (practiceCentre is null)
        {
            return Result.Failure<List<DayAvailabilityResponse>>(
                new Error("Appointment.PracticeCentreNotFound",
                    "The specified practice centre was not found.",
                    ErrorType.NotFound));
        }

        // Build set of active days-of-week from session groups
        var activeDays = practiceCentre.SessionGroups
            .SelectMany(sg => sg.DaysOfWeek)
            .Select(d => d.ToUpperInvariant())
            .ToHashSet();

        // Collect all dates in range that match active days
        var availableDates = new List<DateOnly>();
        for (var d = request.From; d <= request.To; d = d.AddDays(1))
        {
            var dayAbbr = d.DayOfWeek.ToString()[..3].ToUpperInvariant();
            if (activeDays.Contains(dayAbbr))
            {
                availableDates.Add(d);
            }
        }

        if (availableDates.Count == 0)
        {
            return new List<DayAvailabilityResponse>();
        }

        // Count existing tickets per date in one query
        var fromDateTime = request.From.ToDateTime(TimeOnly.MinValue);
        var toDateTime = request.To.ToDateTime(TimeOnly.MaxValue);

        var ticketCounts = await dbContext.PatientQueueTickets
            .AsNoTracking()
            .Where(t =>
                t.PracticeCentreId == request.PracticeCentreId &&
                t.DoctorId == request.DoctorAccountId &&
                t.VisitDate >= fromDateTime &&
                t.VisitDate <= toDateTime)
            .GroupBy(t => t.VisitDate.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var countMap = ticketCounts.ToDictionary(
            x => DateOnly.FromDateTime(x.Date),
            x => x.Count);

        var result = availableDates.Select(date =>
        {
            var booked = countMap.TryGetValue(date, out var c) ? c : 0;
            var maxPatients = practiceCentre.MaxPatients;
            var isFull = maxPatients.HasValue && booked >= maxPatients.Value;
            return new DayAvailabilityResponse(date, maxPatients, booked, isFull);
        }).ToList();

        return result;
    }
}
