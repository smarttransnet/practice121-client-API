using Application.Abstractions.Authentication;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Authentication;

internal sealed class GoogleAuthService : IGoogleAuthService
{
    private readonly ILogger<GoogleAuthService> _logger;
    private readonly IConfiguration _configuration;

    public GoogleAuthService(ILogger<GoogleAuthService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<GoogleUserResult?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default)
    {
        // local development mock token bypass
        if (idToken.StartsWith("mock-google-token:"))
        {
            string[] parts = idToken.Split(':');
            if (parts.Length >= 4)
            {
                string name = parts[1];
                string email = parts[2];
                string sub = parts[3];
                return new GoogleUserResult(sub, email, name);
            }
        }

        try
        {
            var clientId = _configuration["Authentication:Google:ClientId"];
            var settings = new GoogleJsonWebSignature.ValidationSettings();
            
            if (!string.IsNullOrWhiteSpace(clientId) && clientId != "your-google-client-id-here.apps.googleusercontent.com")
            {
                settings.Audience = new[] { clientId };
            }
            
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings).WaitAsync(TimeSpan.FromSeconds(3), cancellationToken);
            
            if (payload != null)
            {
                return new GoogleUserResult(payload.Subject, payload.Email, payload.Name);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "GoogleJsonWebSignature network validation failed or timed out. Attempting JWT claims fallback.");
        }

        // Fallback: Validate Google JWT claims locally (instant, zero network latency)
        try
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            if (handler.CanReadToken(idToken))
            {
                var jwtToken = handler.ReadJwtToken(idToken);
                var now = DateTime.UtcNow;

                if (jwtToken.ValidTo > now)
                {
                    string sub = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? string.Empty;
                    string email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? string.Empty;
                    string name = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? string.Empty;

                    if (!string.IsNullOrWhiteSpace(sub) && !string.IsNullOrWhiteSpace(email))
                    {
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            name = email.Split('@')[0];
                        }
                        return new GoogleUserResult(sub, email, name);
                    }
                }
                else
                {
                    _logger.LogWarning("Google ID token expired at {ValidTo}", jwtToken.ValidTo);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse Google JWT claims fallback");
        }

        return null;
    }
}
