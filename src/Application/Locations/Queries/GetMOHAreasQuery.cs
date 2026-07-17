using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Locations.Queries;

public record GetMohAreasQuery(Guid DistrictId) : IQuery<List<MohAreaResponse>>;

public record MohAreaResponse(Guid Id, string Name);

internal sealed class GetMohAreasQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetMohAreasQuery, List<MohAreaResponse>>
{
    public async Task<Result<List<MohAreaResponse>>> Handle(GetMohAreasQuery request, CancellationToken cancellationToken)
    {
        var mohAreas = await dbContext.MohAreas
            .AsNoTracking()
            .Where(m => m.DistrictId == request.DistrictId)
            .OrderBy(m => m.Name)
            .Select(m => new MohAreaResponse(m.Id, m.Name))
            .ToListAsync(cancellationToken);

        return mohAreas;
    }
}
