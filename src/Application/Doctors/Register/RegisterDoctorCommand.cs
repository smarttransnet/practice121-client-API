using Application.Abstractions.Messaging;

namespace Application.Doctors.Register;

public sealed record RegisterDoctorResult(Guid AccountId, Guid OtpSessionId);

public sealed record RegisterDoctorCommand(string Name, string Email, string Password)
    : ICommand<RegisterDoctorResult>;
