using Application.Abstractions.Messaging;

namespace Application.Doctors.Login;

public sealed record LoginDoctorResult(Guid AccountId, Guid OtpSessionId);

public sealed record LoginDoctorCommand(string Email, string Password)
    : ICommand<LoginDoctorResult>;
