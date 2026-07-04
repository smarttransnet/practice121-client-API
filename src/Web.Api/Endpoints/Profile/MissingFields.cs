using Application.Abstractions.Messaging;
using Application.Doctors.GetMissingFields;
using SharedKernel;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Profile;

internal sealed class MissingFields : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/profile/missing-fields", async (
            IQueryHandler<GetMissingFieldsQuery, MissingFieldsResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetMissingFieldsQuery();

            Result<MissingFieldsResponse> result = await handler.Handle(query, cancellationToken);

            return result.ToApiResponse();
        })
        .RequireAuthorization()
        .WithTags(Tags.Profile);
    }
}
