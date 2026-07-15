using Application.Abstractions.Messaging;

namespace Application.Favorites.Create;

public sealed class CreateFavoriteMedicineCommand : ICommand<Guid>
{
    public string VerifiedName { get; set; }
    public string Category { get; set; }
}
