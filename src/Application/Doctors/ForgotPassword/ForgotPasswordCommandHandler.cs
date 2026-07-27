using Application.Abstractions;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;
using System.Security.Cryptography;

namespace Application.Doctors.ForgotPassword;

internal sealed class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly AppSettings _appSettings;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    public ForgotPasswordCommandHandler(
        IApplicationDbContext context,
        IEmailService emailService,
        IPasswordHasher passwordHasher,
        IDateTimeProvider dateTimeProvider,
        AppSettings appSettings,
        ILogger<ForgotPasswordCommandHandler> logger)
    {
        _context = context;
        _emailService = emailService;
        _passwordHasher = passwordHasher;
        _dateTimeProvider = dateTimeProvider;
        _appSettings = appSettings;
        _logger = logger;
    }

    public async Task<Result<string>> Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        const string genericMessage = "If this email is registered, a password reset link has been sent.";

        string emailNormalized = command.Email.ToLowerInvariant().Trim();

        // Lookup account — result is intentionally NOT exposed to prevent user enumeration
        DoctorAccount? account = await _context.DoctorAccounts
            .SingleOrDefaultAsync(a => a.Email == emailNormalized, cancellationToken);

        if (account is null || account.AuthProvider == AuthProvider.GOOGLE || string.IsNullOrEmpty(account.PasswordHash))
        {
            // Still return success — never reveal whether email exists
            _logger.LogInformation("Forgot password requested for email {Email}; account not found or non-local auth.", emailNormalized);
            return genericMessage;
        }

        // Generate a cryptographically secure 32-byte random token
        byte[] tokenBytes = RandomNumberGenerator.GetBytes(32);
        string rawToken = Convert.ToBase64String(tokenBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", ""); // URL-safe Base64

        // Hash the token before storing — reuse IPasswordHasher (BCrypt) so Application stays BCrypt-free
        string tokenHash = _passwordHasher.Hash(rawToken);

        int expiryMinutes = _appSettings.PasswordResetTokenExpiryMinutes;
        account.PasswordResetTokenHash = tokenHash;
        account.PasswordResetTokenExpiry = _dateTimeProvider.UtcNow.AddMinutes(expiryMinutes);

        await _context.SaveChangesAsync(cancellationToken);

        // Build reset URL — never log the raw token
        string resetUrl = $"{_appSettings.BaseUrl}/#/reset-password?accountId={account.Id}&token={Uri.EscapeDataString(rawToken)}";

        string subject = "Doctor Portal — Password Reset Request";
        string body = $"""
            Hello,

            We received a request to reset the password for your Doctor Portal account ({account.Email}).

            Click the link below to set a new password. This link is valid for {expiryMinutes} minutes and can only be used once.

            {resetUrl}

            If you did not request a password reset, please ignore this email. Your password will not be changed.

            — Practice121 Team
            """;

        await _emailService.SendEmailAsync(account.Email, subject, body, cancellationToken);

        _logger.LogInformation("Password reset email dispatched for account {AccountId}.", account.Id);

        return genericMessage;
    }
}
