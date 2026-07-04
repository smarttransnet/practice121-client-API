namespace Domain.Doctors;

public enum AuthProvider
{
    LOCAL = 1,
    GOOGLE = 2
}

public enum AccountStatus
{
    PENDING = 1,
    ACTIVE = 2,
    SUSPENDED = 3
}

public enum ProfileCompletionStatus
{
    MINIMAL = 1,
    PARTIAL = 2,
    COMPLETE = 3
}

public enum DocumentType
{
    NIC = 1,
    SLMC_CERT = 2,
    OTHER = 3
}

public enum DocumentStatus
{
    PENDING = 1,
    APPROVED = 2,
    REJECTED = 3
}

public enum OtpChannel
{
    MOBILE = 1,
    EMAIL = 2
}

public enum MedicalSpecialty
{
    Cardiology = 1,
    Neurology = 2,
    Pediatrics = 3,
    Dermatology = 4,
    Orthopedics = 5,
    GeneralPractice = 6,
    Oncology = 7,
    Psychiatry = 8
}
