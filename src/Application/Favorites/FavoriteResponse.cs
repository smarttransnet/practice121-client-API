namespace Application.Favorites;

public sealed class FavoriteResponse
{
    public Guid Id { get; set; }
    public string VerifiedName { get; set; }
    public string Category { get; set; }
    public DateTime CreatedAt { get; set; }
}
