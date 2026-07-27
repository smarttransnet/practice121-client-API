using Application.Abstractions.Messaging;

namespace Application.Doctors.ResetPassword;

public sealed record ResetPasswordCommand(
    string AccountId,
    string Token,
    string NewPassword,
    string ConfirmPassword) : ICommand<string>;
