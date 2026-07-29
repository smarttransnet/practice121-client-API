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
        string htmlBody = $"""
            <div style="font-family: Arial, sans-serif; max-width: 550px; margin: 0 auto; padding: 24px; border: 1px solid #e2e8f0; border-radius: 12px; background-color: #ffffff;">
                <div style="text-align: center; margin-bottom: 20px;">
                    <h2 style="color: #1976d2; margin: 0; font-size: 22px;">Doctor Portal</h2>
                    <p style="color: #64748b; font-size: 14px; margin-top: 4px;">Password Reset Request</p>
                </div>
                <p style="color: #334155; font-size: 15px; line-height: 1.5;">Hello,</p>
                <p style="color: #334155; font-size: 15px; line-height: 1.5;">We received a request to reset the password for your account (<strong>{account.Email}</strong>).</p>
                <p style="color: #334155; font-size: 15px; line-height: 1.5;">Click the button below to set a new password. This link is valid for <strong>{expiryMinutes} minutes</strong> and can only be used once:</p>
                <div style="text-align: center; margin: 28px 0;">
                    <a href="{resetUrl}" target="_blank" style="background-color: #1976d2; color: #ffffff; padding: 14px 28px; text-decoration: none; font-weight: bold; border-radius: 8px; font-size: 16px; display: inline-block; box-shadow: 0 4px 10px rgba(25, 118, 210, 0.3);">
                        Reset My Password
                    </a>
                </div>
                <p style="color: #64748b; font-size: 13px; line-height: 1.5;">If the button above doesn't work, copy and paste this URL into your browser:</p>
                <p style="color: #1976d2; font-size: 12px; word-break: break-all;"><a href="{resetUrl}" style="color: #1976d2;">{resetUrl}</a></p>
                <p style="color: #64748b; font-size: 13px; line-height: 1.5; margin-top: 20px;">If you did not request a password reset, please ignore this email or contact support. Your password will remain unchanged.</p>
                <hr style="border: none; border-top: 1px solid #e2e8f0; margin: 24px 0;" />
                <p style="color: #94a3b8; font-size: 12px; text-align: center; margin: 0;">Practice121 Medical Professional Network</p>
            </div>
            """;

        // EMAIL SENDING DISABLED: Re-evaluate email call site here if needed.
        bool enableEmailSending = false;
        if (enableEmailSending)
        {
            await _emailService.SendEmailAsync(account.Email, subject, htmlBody, cancellationToken);
        }
        else
        {
            _logger.LogInformation("[DISABLED] Password reset email dispatch skipped for account {AccountId}. Subject: '{Subject}'", account.Id, subject);
        }

        return genericMessage;
    }
}
