using Application.Abstractions.Messaging;
using Application.Doctors.VerifyOtp;

namespace Application.Doctors.Login;

public sealed record LoginDoctorCommand(string Email, string Password)
    : ICommand<TokenResponse>;

