using Domain.Doctors;
using SharedKernel;

namespace Domain.Patients;

public sealed class PatientAccount : Entity
{
    public Guid Id { get; set; }
    public string NicNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string MobileNumber { get; set; } = string.Empty;
    
    public bool Verified { get; set; }
    public ProfileCompletionStatus CompletionStatus { get; set; }
    
    // Optional linkage to the doctor who created this profile
    public Guid? CreatedByDoctorId { get; set; }
    public DoctorAccount? CreatedByDoctor { get; set; }

    public ICollection<PatientDocument> Documents { get; set; } = [];
}
