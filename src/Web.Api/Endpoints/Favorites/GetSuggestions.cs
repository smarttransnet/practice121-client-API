using Application.Abstractions.Messaging;
using Application.Favorites.Suggestions;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Favorites;

internal sealed class GetSuggestions : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/favorites/suggestions", async (
            string? query,
            IQueryHandler<GetSmartSuggestionsQuery, List<FavoriteSuggestionResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var queryCommand = new GetSmartSuggestionsQuery(query);
            Result<List<FavoriteSuggestionResponse>> result = await handler.Handle(queryCommand, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Favorites)
        .RequireAuthorization();
    }
}
