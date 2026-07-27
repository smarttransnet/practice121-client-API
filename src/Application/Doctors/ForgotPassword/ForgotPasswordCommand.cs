using Application.Abstractions.Messaging;

namespace Application.Doctors.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : ICommand<string>;
