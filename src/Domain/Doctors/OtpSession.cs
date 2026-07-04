using SharedKernel;

namespace Domain.Doctors;

public sealed class OtpSession : Entity
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public OtpChannel Channel { get; set; }
    public string OtpHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool Verified { get; set; }
    public int Attempts { get; set; }

    // Navigation properties
    public DoctorAccount? Account { get; set; }
}
