using Application.Abstractions.Messaging;

namespace Application.Favorites.Update;

public sealed record UpdateFavoriteMedicineCommand(
    Guid Id,
    string GenericName,
    string? BrandName,
    string Category,
    string? Dose,
    string? Frequency,
    string? Duration) : ICommand;
