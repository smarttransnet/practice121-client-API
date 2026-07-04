using Application.Abstractions.Messaging;
using Domain.Doctors;

namespace Application.Doctors.ResendOtp;

public sealed record ResendOtpResult(Guid OtpSessionId);

public sealed record ResendOtpCommand(Guid AccountId, OtpChannel Channel)
    : ICommand<ResendOtpResult>;
