using Application.Abstractions.Messaging;
using Application.Locations.Queries;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Locations;

internal sealed class GetMohAreas : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/locations/moh-areas", async (
            Guid districtId,
            IQueryHandler<GetMohAreasQuery, List<MohAreaResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetMohAreasQuery(districtId);
            Result<List<MohAreaResponse>> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Locations)
        .RequireAuthorization();
    }
}
