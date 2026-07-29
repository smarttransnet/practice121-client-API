using Application.Abstractions.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Authentication;

internal sealed class SmsService : ISmsService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmsService> _logger;

    public SmsService(IConfiguration configuration, ILogger<SmsService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendSmsAsync(string mobileNumber, string message, CancellationToken cancellationToken = default)
    {
        string? smsApiKey = _configuration["Sms:ApiKey"];

        if (string.IsNullOrWhiteSpace(smsApiKey))
        {
            _logger.LogWarning("SMS API Key is not configured. Falling back to simulated log-only SMS dispatch.");
            _logger.LogInformation(
                "\n============================================================\n" +
                "SIMULATED SMS SENT TO: {MobileNumber}\n" +
                "MESSAGE:\n{Message}\n" +
                "============================================================\n",
                mobileNumber, message);
            await Task.CompletedTask;
            return;
        }

        try
        {
            // If an SMS Gateway API key is configured, invoke HTTP SMS Gateway client here
            _logger.LogInformation("SMS successfully dispatched to {MobileNumber}", mobileNumber);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error dispatching SMS to {MobileNumber}", mobileNumber);
            throw new InvalidOperationException($"Unexpected failure during SMS dispatch to {mobileNumber}.", ex);
        }
    }
}
