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
            
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            
            if (payload == null)
            {
                return null;
            }

            return new GoogleUserResult(payload.Subject, payload.Email, payload.Name);
        }
        catch (InvalidJwtException ex)
        {
            _logger.LogWarning(ex, "Failed to validate Google ID token");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error validating Google ID token");
            return null;
        }
    }
}
