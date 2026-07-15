using Application.Abstractions.Messaging;
using Application.Favorites.Create;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Favorites;

internal sealed class Create : IEndpoint
{
    public sealed class Request
    {
        public string VerifiedName { get; set; }
        public string Category { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/favorites", async (
            Request request,
            ICommandHandler<CreateFavoriteMedicineCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateFavoriteMedicineCommand
            {
                VerifiedName = request.VerifiedName,
                Category = request.Category
            };

            Result<Guid> result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Favorites)
        .RequireAuthorization();
    }
}
