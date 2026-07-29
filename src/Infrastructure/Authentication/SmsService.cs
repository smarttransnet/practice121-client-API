using Application.Abstractions.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Infrastructure.Authentication;

internal sealed class SmsService : ISmsService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SmsService> _logger;

    public SmsService(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<SmsService> logger)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task SendSmsAsync(string mobileNumber, string message, CancellationToken cancellationToken = default)
    {
        string? smsApiKey = _configuration["Sms:ApiKey"];
        string senderId = _configuration["Sms:SenderId"] ?? "Practice121";

        if (string.IsNullOrWhiteSpace(smsApiKey) || smsApiKey.Equals("YOUR_SMS_API_KEY", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("SMS API Key is not configured. Falling back to simulated log-only SMS dispatch.");
            _logger.LogInformation(
                "\n============================================================\n" +
                "SIMULATED SMS SENT TO: {MobileNumber}\n" +
                "MESSAGE:\n{Message}\n" +
                "============================================================\n",
                mobileNumber, message);
            return;
        }

        try
        {
            // Parse Notify.lk API Key format: {user_id}|{api_key}
            string userId = "";
            string apiKey = smsApiKey;
            if (smsApiKey.Contains('|'))
            {
                var parts = smsApiKey.Split('|', 2);
                userId = parts[0].Trim();
                apiKey = parts[1].Trim();
            }

            // Format phone number to standard Sri Lanka format (e.g. 0771234567 -> 94771234567)
            string formattedPhone = mobileNumber.Trim().Replace(" ", "").Replace("-", "").Replace("+", "");
            if (formattedPhone.StartsWith('0'))
            {
                formattedPhone = $"94{formattedPhone.AsSpan(1)}";
            }

            using var client = _httpClientFactory.CreateClient();

            // Build request URL for Notify.lk API v1
            var requestUrl = $"https://app.notify.lk/api/v1/send?user_id={Uri.EscapeDataString(userId)}&api_key={Uri.EscapeDataString(apiKey)}&sender_id={Uri.EscapeDataString(senderId)}&to={Uri.EscapeDataString(formattedPhone)}&message={Uri.EscapeDataString(message)}";

            var response = await client.GetAsync(requestUrl, cancellationToken);
            string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("SMS successfully dispatched via Notify.lk to {MobileNumber}. Response: {Response}", mobileNumber, responseBody);
            }
            else
            {
                _logger.LogError("Failed to send SMS via Notify.lk to {MobileNumber}. Status: {Status}, Response: {ResponseBody}", mobileNumber, response.StatusCode, responseBody);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error dispatching SMS to {MobileNumber}", mobileNumber);
        }
    }
}
