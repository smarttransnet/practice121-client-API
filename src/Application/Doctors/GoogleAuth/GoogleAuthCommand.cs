using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Application.Doctors.VerifyOtp;

namespace Application.Doctors.GoogleAuth;

public sealed record GoogleAuthCommand(string IdToken)
    : ICommand<TokenResponse>;
