using Application.Abstractions.Messaging;
using Application.Locations.Queries;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Locations;

internal sealed class GetPlaces : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/locations/places", async (
            Guid mohAreaId,
            IQueryHandler<GetPlacesQuery, List<PlaceResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPlacesQuery(mohAreaId);
            Result<List<PlaceResponse>> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Locations)
        .RequireAuthorization();
    }
}
