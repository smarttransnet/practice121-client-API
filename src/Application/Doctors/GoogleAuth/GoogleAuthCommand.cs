using Application.Abstractions.Messaging;

namespace Application.Doctors.GoogleAuth;

public sealed record GoogleAuthResult(Guid AccountId, Guid OtpSessionId);

public sealed record GoogleAuthCommand(string IdToken)
    : ICommand<GoogleAuthResult>;
