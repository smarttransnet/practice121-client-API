namespace Application.Favorites.Suggestions;

public sealed class FavoriteSuggestionResponse
{
    public string? GenericName { get; set; }
    public string? BrandName { get; set; }
    public string? Category { get; set; }
    public string? Dose { get; set; }
    public string? Frequency { get; set; }
    public string? Duration { get; set; }
    public string? DoctorSpecialty { get; set; }
    public int UsageCount { get; set; }
}
