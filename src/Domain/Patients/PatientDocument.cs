using Domain.Doctors;
using SharedKernel;

namespace Domain.Patients;

public sealed class PatientDocument : Entity
{
    public Guid Id { get; set; }
    public Guid PatientAccountId { get; set; }
    public DocumentType Type { get; set; }
    
    public string? FileUrl { get; set; } // Legacy or external blob
    public byte[]? FileData { get; set; } // Stored as bytes
    public string? ContentType { get; set; }
    
    public bool IsActive { get; set; }
    public DateTime UploadedAt { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public DocumentStatus Status { get; set; }

    public PatientAccount? PatientAccount { get; set; }
}
