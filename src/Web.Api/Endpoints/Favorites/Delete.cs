using Application.Abstractions.Messaging;
using Application.Favorites.Delete;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Favorites;

internal sealed class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/favorites/{id:guid}", async (
            Guid id,
            ICommandHandler<DeleteFavoriteMedicineCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteFavoriteMedicineCommand
            {
                Id = id
            };

            Result result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Favorites)
        .RequireAuthorization();
    }
}
