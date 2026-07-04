using SharedKernel;

namespace Domain.Doctors;

public sealed class Document : Entity
{
    public Guid Id { get; set; }
    public Guid ProfileId { get; set; }
    public DocumentType Type { get; set; }
    public string FileUrl { get; set; } = string.Empty; // Legacy
    public byte[]? FileData { get; set; }
    public string? ContentType { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? ArchivedAt { get; set; }
    public DateTime UploadedAt { get; set; }
    public DocumentStatus Status { get; set; }

    // Navigation properties
    public DoctorProfile? Profile { get; set; }
}
