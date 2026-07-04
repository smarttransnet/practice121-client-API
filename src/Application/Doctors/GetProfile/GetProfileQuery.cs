using Application.Abstractions.Messaging;

namespace Application.Doctors.GetProfile;

public sealed record DocumentDto(
    Guid Id,
    string Type,
    string FileUrl,
    string Status,
    DateTime UploadedAt);

public sealed record SignatureDto(
    string SignatureDataUrl,
    DateTime SignedAt);

public sealed record DoctorProfileResponse(
    Guid AccountId,
    string Email,
    string FullName,
    DateTime? DateOfBirth,
    string? SlmcRegNumber,
    string? NicNumber,
    string? MobileNumber,
    string[] Qualifications,
    string? Specialty,
    string? SubSpecialty,
    string? ProfilePictureUrl,
    string CompletionStatus,
    bool SlmcVerified,
    bool QualificationsVerified,
    bool DocumentsVerified,
    bool BadgeAwarded,
    string? PracticeId,
    string? BarcodeData,
    List<DocumentDto> Documents,
    SignatureDto? ESignature,
    string? FirstName, // ADDED: FirstName — new column, migration required
    string? LastName, // ADDED: LastName — new column, migration required
    string? Gender, // ADDED: Gender — new column, migration required
    string? Bio); // ADDED: Bio — new column, migration required

public sealed record GetProfileQuery : IQuery<DoctorProfileResponse>;
