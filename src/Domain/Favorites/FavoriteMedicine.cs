using SharedKernel;

namespace Domain.Favorites;

public sealed class FavoriteMedicine : Entity
{
    public Guid Id { get; set; }
    public Guid DoctorId { get; set; }
    public string VerifiedName { get; set; }
    public string Category { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
