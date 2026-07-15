using Application.Abstractions.Messaging;

namespace Application.Favorites.Update;

public sealed class UpdateFavoriteMedicineCommand : ICommand
{
    public Guid Id { get; set; }
    public string VerifiedName { get; set; }
    public string Category { get; set; }
}
