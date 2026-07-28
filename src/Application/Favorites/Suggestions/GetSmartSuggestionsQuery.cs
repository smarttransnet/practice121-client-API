using Application.Abstractions.Messaging;

namespace Application.Favorites.Suggestions;

public sealed record GetSmartSuggestionsQuery(string? Query) : IQuery<List<FavoriteSuggestionResponse>>;
