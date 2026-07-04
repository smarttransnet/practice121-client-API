using Application.Abstractions.Messaging;
using Application.Doctors.VerifyOtp;

namespace Application.Doctors.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken)
    : ICommand<TokenResponse>;
