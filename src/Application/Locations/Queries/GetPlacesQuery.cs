using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Locations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Locations.Queries;

public record GetPlacesQuery(Guid MohAreaId) : IQuery<List<PlaceResponse>>;

public record PlaceResponse(Guid Id, string Name, bool IsVerified);

internal sealed class GetPlacesQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetPlacesQuery, List<PlaceResponse>>
{
    public async Task<Result<List<PlaceResponse>>> Handle(GetPlacesQuery request, CancellationToken cancellationToken)
    {
        var places = await dbContext.Places
            .AsNoTracking()
            .Where(p => p.MohAreaId == request.MohAreaId)
            .OrderBy(p => p.Name)
            .Select(p => new PlaceResponse(p.Id, p.Name, p.IsVerified))
            .ToListAsync(cancellationToken);

        return places;
    }
}
