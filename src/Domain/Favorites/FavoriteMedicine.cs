using SharedKernel;

namespace Domain.Favorites;

public sealed class FavoriteMedicine : Entity
{
    public Guid Id { get; set; }
    public Guid DoctorId { get; set; }
    public string? GenericName { get; set; }
    public string? BrandName { get; set; }
    public string? Category { get; set; }
    public string? Dose { get; set; }
    public string? Frequency { get; set; }
    public string? Duration { get; set; }
    public string? DoctorSpecialty { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
