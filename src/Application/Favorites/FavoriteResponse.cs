namespace Application.Favorites;

public sealed class FavoriteResponse
{
    public Guid Id { get; set; }
    public string GenericName { get; set; } = string.Empty;
    public string? BrandName { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Dose { get; set; }
    public string? Frequency { get; set; }
    public string? Duration { get; set; }
    public string? DoctorSpecialty { get; set; }
    public DateTime CreatedAt { get; set; }
}
