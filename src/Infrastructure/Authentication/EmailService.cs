using Application.Abstractions.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastructure.Authentication;

internal sealed class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        string? apiKey = _configuration["SendGrid:ApiKey"];
        string fromEmail = _configuration["SendGrid:FromEmail"] ?? "tharaka@practice121.com";
        string fromName = _configuration["SendGrid:FromName"] ?? "Practice121 Doctor Portal";

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("SendGrid API Key is not configured. Falling back to simulated log-only email dispatch.");
            _logger.LogInformation(
                "\n============================================================\n" +
                "SIMULATED EMAIL SENT TO: {To}\n" +
                "SUBJECT: {Subject}\n" +
                "BODY:\n{Body}\n" +
                "============================================================\n",
                to, subject, body);
            return;
        }

        try
        {
            var client = new SendGridClient(apiKey);
            var fromAddress = new EmailAddress(fromEmail, fromName);
            var toAddress = new EmailAddress(to);

            bool isHtml = body.Contains("<html", StringComparison.OrdinalIgnoreCase) ||
                         body.Contains("<p>", StringComparison.OrdinalIgnoreCase) ||
                         body.Contains("<div>", StringComparison.OrdinalIgnoreCase) ||
                         body.Contains("<br", StringComparison.OrdinalIgnoreCase);

            string plainTextContent = isHtml ? System.Text.RegularExpressions.Regex.Replace(body, "<.*?>", string.Empty) : body;
            string htmlContent = isHtml ? body : body.Replace("\n", "<br/>");

            var msg = MailHelper.CreateSingleEmail(fromAddress, toAddress, subject, plainTextContent, htmlContent);

            var response = await client.SendEmailAsync(msg, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Email successfully sent via SendGrid to {To} with Subject: '{Subject}' (Status: {StatusCode})", to, subject, response.StatusCode);
            }
            else
            {
                string errorResponseBody = await response.Body.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Failed to send email via SendGrid to {To}. Status: {Status}, Response: {ResponseBody}", to, response.StatusCode, errorResponseBody);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error dispatching email via SendGrid to {To}", to);
            throw new InvalidOperationException($"Unexpected failure during SendGrid email dispatch to {to}.", ex);
        }
    }
}
