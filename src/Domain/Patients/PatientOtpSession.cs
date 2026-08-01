using SharedKernel;

namespace Domain.Patients;

public sealed class PatientOtpSession : Entity
{
    public Guid Id { get; set; }
    public string MobileNumber { get; set; } = string.Empty;
    public string OtpHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool Verified { get; set; }
    public string? VerificationToken { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public int Attempts { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastSentAt { get; set; }
}
