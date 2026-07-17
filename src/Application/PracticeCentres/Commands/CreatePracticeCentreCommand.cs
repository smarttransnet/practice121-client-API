using System.Globalization;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.PracticeCentres;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.PracticeCentres.Commands;

public record CreatePracticeCentreCommand(
    Guid DoctorId,
    string ClinicName,
    Guid PlaceId,
    int? MaxPatients,
    List<SessionGroupDto> SessionGroups,
    List<NurseDto> Nurses) : ICommand<Guid>;

public record SessionGroupDto(
    List<string> DaysOfWeek,
    List<TimeBlockDto> TimeBlocks);

public record TimeBlockDto(
    string Label,
    string StartTime,
    string EndTime);

public record NurseDto(
    string Name,
    string PhoneNumber,
    bool IsActive);

internal sealed class CreatePracticeCentreCommandHandler(IApplicationDbContext dbContext)
    : ICommandHandler<CreatePracticeCentreCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreatePracticeCentreCommand request, CancellationToken cancellationToken)
    {
        // Place validation
        var placeExists = await dbContext.Places.AnyAsync(p => p.Id == request.PlaceId, cancellationToken);
        if (!placeExists)
        {
            return Result.Failure<Guid>(new Error("PracticeCentre.PlaceNotFound", "The specified Place does not exist.", ErrorType.NotFound));
        }

        // Parse new sessions
        var newSessions = new List<(string Day, TimeSpan Start, TimeSpan End, string ClinicName)>();
        foreach (var sg in request.SessionGroups)
        {
            foreach (var day in sg.DaysOfWeek)
            {
                foreach (var tb in sg.TimeBlocks)
                {
                    var start = TimeSpan.Parse(tb.StartTime, CultureInfo.InvariantCulture);
                    var end = TimeSpan.Parse(tb.EndTime, CultureInfo.InvariantCulture);
                    newSessions.Add((day, start, end, request.ClinicName));
                }
            }
        }

        // Fetch all practice centres for the doctor to check for overlaps
        var existingCentres = await dbContext.PracticeCentres
            .AsNoTracking()
            .Include(pc => pc.SessionGroups)
                .ThenInclude(sg => sg.TimeBlocks)
            .Where(pc => pc.DoctorId == request.DoctorId)
            .ToListAsync(cancellationToken);

        var existingSessions = new List<(string Day, TimeSpan Start, TimeSpan End, string ClinicName)>();
        foreach (var centre in existingCentres)
        {
            foreach (var sg in centre.SessionGroups)
            {
                foreach (var day in sg.DaysOfWeek)
                {
                    foreach (var tb in sg.TimeBlocks)
                    {
                        existingSessions.Add((day, tb.StartTime, tb.EndTime, centre.ClinicName));
                    }
                }
            }
        }

        // Check for overlaps
        foreach (var newS in newSessions)
        {
            foreach (var extS in existingSessions)
            {
                if (newS.Day == extS.Day && newS.Start < extS.End && newS.End > extS.Start)
                {
                    return Result.Failure<Guid>(new Error("PracticeCentre.Overlap", $"Overlapping session detected on {newS.Day} with {extS.ClinicName}.", ErrorType.Validation));
                }
            }
        }

        // Create the entities
        var practiceCentre = PracticeCentre.Create(request.DoctorId, request.PlaceId, request.ClinicName, request.MaxPatients);

        foreach (var sg in request.SessionGroups)
        {
            var sessionGroup = SessionGroup.Create(practiceCentre.Id, sg.DaysOfWeek);
            foreach (var tb in sg.TimeBlocks)
            {
                var timeBlock = TimeBlock.Create(sessionGroup.Id, tb.Label, TimeSpan.Parse(tb.StartTime, CultureInfo.InvariantCulture), TimeSpan.Parse(tb.EndTime, CultureInfo.InvariantCulture));
                sessionGroup.AddTimeBlock(timeBlock);
            }
            practiceCentre.AddSessionGroup(sessionGroup);
        }

        foreach (var n in request.Nurses)
        {
            var nurse = Nurse.Create(practiceCentre.Id, n.Name, n.PhoneNumber, n.IsActive);
            practiceCentre.AddNurse(nurse);
        }

        dbContext.PracticeCentres.Add(practiceCentre);

        await dbContext.SaveChangesAsync(cancellationToken);

        return practiceCentre.Id;
    }
}
