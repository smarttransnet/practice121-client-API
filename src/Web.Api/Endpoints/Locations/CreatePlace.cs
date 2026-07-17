using Application.Abstractions.Messaging;
using Application.Locations.Commands;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Locations;

internal sealed class CreatePlace : IEndpoint
{
    public sealed record Request(Guid MohAreaId, string Name);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/locations/places", async (
            Request request,
            ICommandHandler<CreatePlaceCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreatePlaceCommand(request.MohAreaId, request.Name);
            Result<Guid> result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Locations)
        .RequireAuthorization();
    }
}
