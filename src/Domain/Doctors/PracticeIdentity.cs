using SharedKernel;

namespace Domain.Doctors;

public sealed class PracticeIdentity : Entity
{
    public string PracticeId { get; set; } = string.Empty;
    public Guid AccountId { get; set; }
    public string BarcodeData { get; set; } = string.Empty;

    // Navigation properties
    public DoctorAccount? Account { get; set; }
}
