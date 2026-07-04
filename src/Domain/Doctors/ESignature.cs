using SharedKernel;

namespace Domain.Doctors;

public sealed class ESignature : Entity
{
    public Guid Id { get; set; }
    public Guid ProfileId { get; set; }
    public string SignatureDataUrl { get; set; } = string.Empty; // Legacy
    public byte[]? SignatureData { get; set; }
    public string? SignatureContentType { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? ArchivedAt { get; set; }
    public DateTime SignedAt { get; set; }
    public string IpAddress { get; set; } = string.Empty;

    // Navigation properties
    public DoctorProfile? Profile { get; set; }
}
