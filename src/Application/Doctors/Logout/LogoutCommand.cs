using Application.Abstractions.Messaging;

namespace Application.Doctors.Logout;

public sealed record LogoutCommand(string RefreshToken)
    : ICommand;
