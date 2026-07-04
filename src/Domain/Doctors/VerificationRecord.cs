using SharedKernel;

namespace Domain.Doctors;

public sealed class VerificationRecord : Entity
{
    public Guid Id { get; set; }
    public Guid ProfileId { get; set; }
    public bool SlmcVerified { get; set; }
    public bool QualificationsVerified { get; set; }
    public bool DocumentsVerified { get; set; }
    public bool BadgeAwarded { get; set; }
    public Guid? AuditedBy { get; set; }
    public DateTime? VerifiedAt { get; set; }

    // Navigation properties
    public DoctorProfile? Profile { get; set; }
}
