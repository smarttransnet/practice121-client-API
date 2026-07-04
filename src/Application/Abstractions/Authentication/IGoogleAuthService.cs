namespace Application.Abstractions.Authentication;

public record GoogleUserResult(string Sub, string Email, string Name);

public interface IGoogleAuthService
{
    Task<GoogleUserResult?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default);
}
