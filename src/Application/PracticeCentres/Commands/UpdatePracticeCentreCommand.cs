using System.Globalization;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.PracticeCentres;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.PracticeCentres.Commands;

public record UpdatePracticeCentreCommand(
    Guid Id,
    Guid DoctorId,
    string ClinicName,
    Guid PlaceId,
    int? MaxPatients,
    List<SessionGroupDto> SessionGroups,
    List<NurseDto> Nurses) : ICommand;

internal sealed class UpdatePracticeCentreCommandHandler(IApplicationDbContext dbContext)
    : ICommandHandler<UpdatePracticeCentreCommand>
{
    public async Task<Result> Handle(UpdatePracticeCentreCommand request, CancellationToken cancellationToken)
    {
        var centre = await dbContext.PracticeCentres
            .Include(pc => pc.SessionGroups)
            .Include(pc => pc.Nurses)
            .FirstOrDefaultAsync(pc => pc.Id == request.Id && pc.DoctorId == request.DoctorId, cancellationToken);

        if (centre == null)
        {
            return Result.Failure(new Error("PracticeCentre.NotFound", "The specified Practice Centre was not found or you do not have permission to edit it.", ErrorType.NotFound));
        }

        var placeExists = await dbContext.Places.AnyAsync(p => p.Id == request.PlaceId, cancellationToken);
        if (!placeExists)
        {
            return Result.Failure(new Error("PracticeCentre.PlaceNotFound", "The specified Place does not exist.", ErrorType.NotFound));
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

        // Fetch all OTHER practice centres for the doctor to check for overlaps
        var existingCentres = await dbContext.PracticeCentres
            .AsNoTracking()
            .Include(pc => pc.SessionGroups)
                .ThenInclude(sg => sg.TimeBlocks)
            .Where(pc => pc.DoctorId == request.DoctorId && pc.Id != request.Id)
            .ToListAsync(cancellationToken);

        var existingSessions = new List<(string Day, TimeSpan Start, TimeSpan End, string ClinicName)>();
        foreach (var existingCentre in existingCentres)
        {
            foreach (var sg in existingCentre.SessionGroups)
            {
                foreach (var day in sg.DaysOfWeek)
                {
                    foreach (var tb in sg.TimeBlocks)
                    {
                        existingSessions.Add((day, tb.StartTime, tb.EndTime, existingCentre.ClinicName));
                    }
                }
            }
        }

        // Check for overlaps with other centres
        foreach (var newS in newSessions)
        {
            foreach (var extS in existingSessions)
            {
                if (newS.Day == extS.Day && newS.Start < extS.End && newS.End > extS.Start)
                {
                    return Result.Failure(new Error("PracticeCentre.Overlap", $"Overlapping session detected on {newS.Day} with {extS.ClinicName}.", ErrorType.Validation));
                }
            }
        }

        // Update properties
        centre.Update(request.PlaceId, request.ClinicName, request.MaxPatients);

        // Replace session groups
        dbContext.SessionGroups.RemoveRange(centre.SessionGroups);
        centre.ClearSessionGroups();
        
        foreach (var sg in request.SessionGroups)
        {
            var sessionGroup = SessionGroup.Create(centre.Id, sg.DaysOfWeek);
            foreach (var tb in sg.TimeBlocks)
            {
                var timeBlock = TimeBlock.Create(sessionGroup.Id, tb.Label, TimeSpan.Parse(tb.StartTime, CultureInfo.InvariantCulture), TimeSpan.Parse(tb.EndTime, CultureInfo.InvariantCulture));
                sessionGroup.AddTimeBlock(timeBlock);
            }
            centre.AddSessionGroup(sessionGroup);
            dbContext.SessionGroups.Add(sessionGroup);
        }

        // Replace nurses
        dbContext.Nurses.RemoveRange(centre.Nurses);
        centre.ClearNurses();
        
        foreach (var n in request.Nurses)
        {
            var nurse = Nurse.Create(centre.Id, n.Name, n.PhoneNumber, n.IsActive);
            centre.AddNurse(nurse);
            dbContext.Nurses.Add(nurse);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
