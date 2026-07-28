using Application.Abstractions.Messaging;

namespace Application.Favorites.Create;

public sealed record CreateFavoriteMedicineCommand(
    string? GenericName,
    string? BrandName,
    string? Category,
    string? Dose,
    string? Frequency,
    string? Duration) : ICommand<Guid>;
