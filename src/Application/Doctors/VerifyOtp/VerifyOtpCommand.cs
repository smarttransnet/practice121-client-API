using Application.Abstractions.Messaging;

namespace Application.Doctors.VerifyOtp;

public sealed record TokenResponse(
    string AccessToken,
    string RefreshToken,
    string ProfileCompletionStatus,
    Guid AccountId,
    string Email,
    string FullName);

public sealed record VerifyOtpCommand(Guid SessionId, string Code)
    : ICommand<TokenResponse>;
