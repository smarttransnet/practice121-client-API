using SharedKernel;

namespace Domain.Doctors;

public sealed class DoctorProfile : Entity
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? SlmcRegNumber { get; set; }
    public string? NicNumber { get; set; }
    public string? MobileNumber { get; set; }
    public string[] Qualifications { get; set; } = [];
    public string? Specialty { get; set; }
    public string? SubSpecialty { get; set; }
    public string? ProfilePictureUrl { get; set; } // Legacy
    public byte[]? ProfilePictureData { get; set; }
    public string? ProfilePictureContentType { get; set; }
    public string? FirstName { get; set; } // ADDED: FirstName — new column, migration required
    public string? LastName { get; set; } // ADDED: LastName — new column, migration required
    public string? Gender { get; set; } // ADDED: Gender — new column, migration required
    public string? Bio { get; set; } // ADDED: Bio — new column, migration required
    public ProfileCompletionStatus CompletionStatus { get; set; }

    // Navigation properties
    public DoctorAccount? Account { get; set; }
    public VerificationRecord? VerificationRecord { get; set; }
    public ICollection<Document> Documents { get; set; } = [];
    public ICollection<Qualification> QualificationsList { get; set; } = [];
    public ESignature? ESignature { get; set; }
}
