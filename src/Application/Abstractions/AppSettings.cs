namespace Application.Abstractions;

/// <summary>
/// Application-level settings read from configuration. Populated by Infrastructure.
/// </summary>
public sealed class AppSettings
{
    public string BaseUrl { get; set; } = "http://localhost:5173";
    public int PasswordResetTokenExpiryMinutes { get; set; } = 60;
}
