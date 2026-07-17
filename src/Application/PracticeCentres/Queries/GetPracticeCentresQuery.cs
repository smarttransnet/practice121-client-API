using System.Globalization;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.PracticeCentres.Queries;

public record GetPracticeCentresQuery(Guid DoctorId) : IQuery<List<PracticeCentreResponse>>;

public record PracticeCentreResponse(
    Guid Id,
    string ClinicName,
    string DistrictName,
    string MohAreaName,
    string PlaceName,
    int? MaxPatients,
    List<SessionGroupResponse> SessionGroups,
    List<NurseResponse> Nurses);

public record SessionGroupResponse(
    Guid Id,
    List<string> DaysOfWeek,
    List<TimeBlockResponse> TimeBlocks);

public record TimeBlockResponse(
    Guid Id,
    string Label,
    string StartTime,
    string EndTime);

public record NurseResponse(
    Guid Id,
    string Name,
    string PhoneNumber,
    bool IsActive);

internal sealed class GetPracticeCentresQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetPracticeCentresQuery, List<PracticeCentreResponse>>
{
    public async Task<Result<List<PracticeCentreResponse>>> Handle(GetPracticeCentresQuery request, CancellationToken cancellationToken)
    {
        var centres = await dbContext.PracticeCentres
            .AsNoTracking()
            .Include(pc => pc.Place)
                .ThenInclude(p => p.MohArea)
                    .ThenInclude(m => m.District)
            .Include(pc => pc.SessionGroups)
                .ThenInclude(sg => sg.TimeBlocks)
            .Include(pc => pc.Nurses)
            .Where(pc => pc.DoctorId == request.DoctorId)
            .ToListAsync(cancellationToken);

        return centres.Select(pc => new PracticeCentreResponse(
            pc.Id,
            pc.ClinicName,
            pc.Place.MohArea.District.Name,
            pc.Place.MohArea.Name,
            pc.Place.Name,
            pc.MaxPatients,
            pc.SessionGroups.Select(sg => new SessionGroupResponse(
                sg.Id,
                sg.DaysOfWeek.ToList(),
                sg.TimeBlocks.Select(tb => new TimeBlockResponse(tb.Id, tb.Label, tb.StartTime.ToString(@"hh\:mm", CultureInfo.InvariantCulture), tb.EndTime.ToString(@"hh\:mm", CultureInfo.InvariantCulture))).ToList()
            )).ToList(),
            pc.Nurses.Select(n => new NurseResponse(n.Id, n.Name, n.PhoneNumber, n.IsActive)).ToList()
        )).ToList();
    }
}
