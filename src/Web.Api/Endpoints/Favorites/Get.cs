using Application.Abstractions.Messaging;
using Application.Favorites;
using Application.Favorites.Get;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Favorites;

internal sealed class Get : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/favorites", async (
            IQueryHandler<GetFavoriteMedicinesQuery, List<FavoriteResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetFavoriteMedicinesQuery();
            Result<List<FavoriteResponse>> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Favorites)
        .RequireAuthorization();
    }
}
