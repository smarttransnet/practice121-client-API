using Application.Abstractions.Authentication;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Authentication;

internal sealed class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "\n" +
            "============================================================\n" +
            "SIMULATED EMAIL SENT TO: {To}\n" +
            "SUBJECT: {Subject}\n" +
            "BODY:\n{Body}\n" +
            "============================================================\n",
            to, subject, body);

        return Task.CompletedTask;
    }
}
