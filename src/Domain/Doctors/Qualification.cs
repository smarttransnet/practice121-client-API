using SharedKernel;

namespace Domain.Doctors;

public sealed class Qualification : Entity
{
    public Guid Id { get; set; }
    public Guid ProfileId { get; set; }
    public string Name { get; set; } = string.Empty;
    public byte[]? CertificateData { get; set; }
    public string? CertificateContentType { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? ArchivedAt { get; set; }

    public DoctorProfile? Profile { get; set; }
}
