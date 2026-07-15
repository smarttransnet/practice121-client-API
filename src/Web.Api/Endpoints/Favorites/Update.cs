using Application.Abstractions.Messaging;
using Application.Favorites.Update;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Favorites;

internal sealed class Update : IEndpoint
{
    public sealed class Request
    {
        public string VerifiedName { get; set; }
        public string Category { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/favorites/{id:guid}", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateFavoriteMedicineCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateFavoriteMedicineCommand
            {
                Id = id,
                VerifiedName = request.VerifiedName,
                Category = request.Category
            };

            Result result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Favorites)
        .RequireAuthorization();
    }
}
