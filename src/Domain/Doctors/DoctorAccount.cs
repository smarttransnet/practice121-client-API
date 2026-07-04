using SharedKernel;

namespace Domain.Doctors;

public sealed class DoctorAccount : Entity
{
    public Guid Id { get; set; }
    public AuthProvider AuthProvider { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public string? GoogleSubId { get; set; }
    public AccountStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }

    // Navigation properties
    public DoctorProfile? Profile { get; set; }
    public PracticeIdentity? PracticeIdentity { get; set; }
    public ICollection<OtpSession> OtpSessions { get; set; } = [];
}
