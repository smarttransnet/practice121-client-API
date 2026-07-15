using Application.Abstractions.Messaging;

namespace Application.Favorites.Delete;

public sealed class DeleteFavoriteMedicineCommand : ICommand
{
    public Guid Id { get; set; }
}
