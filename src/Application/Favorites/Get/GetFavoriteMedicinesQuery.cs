using Application.Abstractions.Messaging;

namespace Application.Favorites.Get;

public sealed class GetFavoriteMedicinesQuery : IQuery<List<FavoriteResponse>>
{
}
