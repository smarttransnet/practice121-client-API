using Application.Abstractions.Messaging;

namespace Application.Patients.Register;

public sealed record RegisterPatientCommand(
    string NicNumber,
    string FirstName,
    string? LastName,
    DateTime? DateOfBirth,
    string? Gender,
    string MobileNumber,
    Guid? CreatedByDoctorId
) : ICommand<Guid>;
