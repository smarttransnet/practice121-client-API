using System.Net.Http.Headers;
using System.Text;
using Application.Abstractions.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Authentication;

internal sealed class EmailService : IEmailService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<EmailService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        string? mailgunApiKey = _configuration["Mailgun:ApiKey"];
        string? mailgunDomain = _configuration["Mailgun:Domain"];
        string? mailgunBaseUrl = _configuration["Mailgun:BaseUrl"];
        string fromEmail = _configuration["Mailgun:FromEmail"] ?? "tharaka@practice121.com";
        string fromName = _configuration["Mailgun:FromName"] ?? "Team Practice121";

        if (!string.IsNullOrWhiteSpace(mailgunApiKey) &&
            mailgunApiKey != "YOUR_MAILGUN_API_KEY" &&
            !string.IsNullOrWhiteSpace(mailgunDomain) &&
            !string.IsNullOrWhiteSpace(mailgunBaseUrl))
        {
            await SendViaMailgunAsync(mailgunApiKey, mailgunDomain, mailgunBaseUrl.TrimEnd('/'), fromEmail, fromName, to, subject, body, cancellationToken);
            return;
        }

        _logger.LogWarning("Mailgun API Key or settings are not configured. Falling back to simulated log-only email dispatch.");
        _logger.LogInformation(
            "\n============================================================\n" +
            "SIMULATED EMAIL SENT TO: {To}\n" +
            "SUBJECT: {Subject}\n" +
            "BODY:\n{Body}\n" +
            "============================================================\n",
            to, subject, body);
    }

    private async Task SendViaMailgunAsync(
        string apiKey,
        string domain,
        string baseUrl,
        string fromEmail,
        string fromName,
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken)
    {
        try
        {
            using var client = _httpClientFactory.CreateClient();
            var authBytes = Encoding.ASCII.GetBytes($"api:{apiKey}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));

            string requestUri = $"{baseUrl}/v3/{domain}/messages";

            using var formData = new MultipartFormDataContent();
            using var fromContent = new StringContent($"{fromName} <{fromEmail}>");
            using var toContent = new StringContent(to);
            using var subjectContent = new StringContent(subject);

            formData.Add(fromContent, "from");
            formData.Add(toContent, "to");
            formData.Add(subjectContent, "subject");

            bool isHtml = body.Contains("<html", StringComparison.OrdinalIgnoreCase) ||
                          body.Contains("<p>", StringComparison.OrdinalIgnoreCase) ||
                          body.Contains("<div>", StringComparison.OrdinalIgnoreCase) ||
                          body.Contains("<br", StringComparison.OrdinalIgnoreCase);

            if (isHtml)
            {
                using var htmlContent = new StringContent(body);
                string plainText = System.Text.RegularExpressions.Regex.Replace(body, "<.*?>", string.Empty);
                using var textContent = new StringContent(plainText);

                formData.Add(htmlContent, "html");
                formData.Add(textContent, "text");

                var response = await client.PostAsync(requestUri, formData, cancellationToken);
                string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Email successfully sent via Mailgun to {To} with Subject: '{Subject}' (Response: {ResponseBody})", to, subject, responseBody);
                }
                else
                {
                    _logger.LogError("Failed to send email via Mailgun to {To}. Status: {Status}, Response: {ResponseBody}", to, response.StatusCode, responseBody);
                    throw new InvalidOperationException($"Mailgun API returned status {response.StatusCode}: {responseBody}");
                }
            }
            else
            {
                using var textContent = new StringContent(body);
                formData.Add(textContent, "text");

                var response = await client.PostAsync(requestUri, formData, cancellationToken);
                string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Email successfully sent via Mailgun to {To} with Subject: '{Subject}' (Response: {ResponseBody})", to, subject, responseBody);
                }
                else
                {
                    _logger.LogError("Failed to send email via Mailgun to {To}. Status: {Status}, Response: {ResponseBody}", to, response.StatusCode, responseBody);
                    throw new InvalidOperationException($"Mailgun API returned status {response.StatusCode}: {responseBody}");
                }
            }
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            _logger.LogError(ex, "Unexpected error dispatching email via Mailgun to {To}", to);
            throw new InvalidOperationException($"Unexpected failure during Mailgun email dispatch to {to}.", ex);
        }
    }
}
