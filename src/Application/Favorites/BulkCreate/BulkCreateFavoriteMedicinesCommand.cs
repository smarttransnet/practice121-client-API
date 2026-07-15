using Application.Abstractions.Messaging;

namespace Application.Favorites.BulkCreate;

public sealed class BulkCreateFavoriteMedicinesCommand : ICommand<int>
{
    public List<FavoriteMedicineDto> Medicines { get; set; } = [];
}

public sealed class FavoriteMedicineDto
{
    public string VerifiedName { get; set; }
    public string Category { get; set; }
}
