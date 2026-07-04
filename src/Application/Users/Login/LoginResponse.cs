namespace Application.Users.Login;

public sealed record LoginResponse(string Token, Guid UserId);
