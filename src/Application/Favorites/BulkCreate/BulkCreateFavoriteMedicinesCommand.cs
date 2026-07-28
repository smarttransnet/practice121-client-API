using Application.Abstractions.Messaging;

namespace Application.Favorites.BulkCreate;

public sealed class BulkCreateFavoriteMedicinesCommand : ICommand<int>
{
    public List<FavoriteMedicineDto> Medicines { get; set; } = [];
}

public sealed class FavoriteMedicineDto
{
    public string? GenericName { get; set; }
    public string? BrandName { get; set; }
    public string? Category { get; set; }
    public string? Dose { get; set; }
    public string? Frequency { get; set; }
    public string? Duration { get; set; }
}
