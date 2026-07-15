using Application.Abstractions.Messaging;
using Application.Favorites.BulkCreate;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Favorites;

internal sealed class BulkCreate : IEndpoint
{
    public sealed class Request
    {
        public List<FavoriteMedicineDto> Medicines { get; set; } = [];
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/favorites/bulk", async (
            Request request,
            ICommandHandler<BulkCreateFavoriteMedicinesCommand, int> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new BulkCreateFavoriteMedicinesCommand
            {
                Medicines = request.Medicines
            };

            Result<int> result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Favorites)
        .RequireAuthorization();
    }
}
