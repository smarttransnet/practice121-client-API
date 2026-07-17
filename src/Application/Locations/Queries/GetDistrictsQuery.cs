using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Locations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Locations.Queries;

public record GetDistrictsQuery : IQuery<List<DistrictResponse>>;

public record DistrictResponse(Guid Id, string Name);

internal sealed class GetDistrictsQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetDistrictsQuery, List<DistrictResponse>>
{
    public async Task<Result<List<DistrictResponse>>> Handle(GetDistrictsQuery request, CancellationToken cancellationToken)
    {
        var districts = await dbContext.Districts
            .AsNoTracking()
            .OrderBy(d => d.Name)
            .Select(d => new DistrictResponse(d.Id, d.Name))
            .ToListAsync(cancellationToken);

        return districts;
    }
}
