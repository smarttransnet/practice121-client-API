using Application.Abstractions.Messaging;
using Application.Locations.Queries;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Locations;

internal sealed class GetDistricts : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/locations/districts", async (
            IQueryHandler<GetDistrictsQuery, List<DistrictResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetDistrictsQuery();
            Result<List<DistrictResponse>> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Locations)
        .RequireAuthorization();
    }
}
