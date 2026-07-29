using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Abstractions.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
        string senderId = _configuration["Sms:SenderId"] ?? "TextLKDemo";

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
            // Format phone number to standard Sri Lanka format (e.g. 0710000000 -> 94710000000)
            string formattedPhone = mobileNumber.Trim().Replace(" ", "").Replace("-", "").Replace("+", "");
            if (formattedPhone.StartsWith('0'))
            {
                formattedPhone = $"94{formattedPhone.AsSpan(1)}";
            }

            using var client = _httpClientFactory.CreateClient();

            // Set Bearer authentication header and JSON accept header
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", smsApiKey.Trim());
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var payload = new
            {
                recipient = formattedPhone,
                sender_id = senderId,
                type = "plain",
                message
            };

            string requestUrl = _configuration["Sms:ApiUrl"] ?? throw new InvalidOperationException("Sms:ApiUrl configuration is missing.");

            var response = await client.PostAsJsonAsync(requestUrl, payload, cancellationToken);
            string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("SMS successfully dispatched via Text.lk v3 to {MobileNumber}. Response: {Response}", mobileNumber, responseBody);
            }
            else
            {
                _logger.LogError("Failed to send SMS via Text.lk v3 to {MobileNumber}. Status: {Status}, Response: {ResponseBody}", mobileNumber, response.StatusCode, responseBody);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error dispatching SMS to {MobileNumber}", mobileNumber);
        }
    }
}
