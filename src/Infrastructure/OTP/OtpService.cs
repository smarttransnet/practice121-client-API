using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Infrastructure.OTP;

internal sealed class OtpService : IOtpService
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<OtpService> _logger;

    public OtpService(
        IApplicationDbContext context,
        IEmailService emailService,
        ISmsService smsService,
        IDateTimeProvider dateTimeProvider,
        ILogger<OtpService> logger)
    {
        _context = context;
        _emailService = emailService;
        _smsService = smsService;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<Guid> GenerateAndSendOtpAsync(
        Guid accountId, 
        string destination, 
        OtpChannel channel, 
        CancellationToken cancellationToken = default)
    {
        // 1. Generate 6-digit numeric OTP
        int code = System.Security.Cryptography.RandomNumberGenerator.GetInt32(100000, 1000000);
        string otpCode = code.ToString(System.Globalization.CultureInfo.InvariantCulture);

        // 2. Hash with BCrypt (cost factor 12)
        string otpHash = BCrypt.Net.BCrypt.HashPassword(otpCode, 12);

        // 3. Create OtpSession
        var session = new OtpSession
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            Channel = channel,
            OtpHash = otpHash,
            ExpiresAt = _dateTimeProvider.UtcNow.AddMinutes(10), // 10 minutes expiry
            Verified = false,
            Attempts = 0
        };

        _context.OtpSessions.Add(session);
        await _context.SaveChangesAsync(cancellationToken);

        // 4. Send verification message in background task for instant API response
        if (channel == OtpChannel.MOBILE)
        {
            string smsText = $"Practice121: Your verification code is : {otpCode}. It expires in 10 minutes. If you didn't request this code, please ignore this message.";
            _ = Task.Run(async () =>
            {
                try
                {
                    await _smsService.SendSmsAsync(destination, smsText, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send background SMS OTP to {Destination}", destination);
                }
            }, CancellationToken.None);
        }
        else
        {
            string subject = "Doctor Portal - Security Verification Code";
            string htmlBody = $"""
                <div style="font-family: Arial, sans-serif; max-width: 550px; margin: 0 auto; padding: 24px; border: 1px solid #e2e8f0; border-radius: 12px; background-color: #ffffff;">
                    <div style="text-align: center; margin-bottom: 20px;">
                        <h2 style="color: #1976d2; margin: 0; font-size: 22px;">Doctor Portal</h2>
                        <p style="color: #64748b; font-size: 14px; margin-top: 4px;">Security Verification Code</p>
                    </div>
                    <p style="color: #334155; font-size: 15px; line-height: 1.5;">Hello,</p>
                    <p style="color: #334155; font-size: 15px; line-height: 1.5;">Please use the following 6-digit verification code to complete your authentication:</p>
                    <div style="text-align: center; margin: 24px 0;">
                        <span style="font-size: 32px; font-weight: bold; letter-spacing: 8px; color: #1976d2; background: #eff6ff; padding: 12px 24px; border-radius: 8px; border: 1px solid #bfdbfe; display: inline-block;">
                            {otpCode}
                        </span>
                    </div>
                    <p style="color: #64748b; font-size: 13px; line-height: 1.5;">This code is valid for <strong>10 minutes</strong>. If you did not initiate this request, please secure your account immediately.</p>
                    <hr style="border: none; border-top: 1px solid #e2e8f0; margin: 24px 0;" />
                    <p style="color: #94a3b8; font-size: 12px; text-align: center; margin: 0;">Practice121 Medical Professional Network</p>
                </div>
                """;
            _ = Task.Run(async () =>
            {
                try
                {
                    // EMAIL SENDING DISABLED: Re-evaluate email call site here if needed.
                    bool enableEmailSending = false;
                    if (enableEmailSending)
                    {
                        await _emailService.SendEmailAsync(destination, subject, htmlBody, CancellationToken.None);
                    }
                    else
                    {
                        _logger.LogInformation("[DISABLED] Skipped sending background Email OTP to {Destination}. Subject: '{Subject}'", destination, subject);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send background Email OTP to {Destination}", destination);
                }
            }, CancellationToken.None);
        }

        return session.Id;
    }

    public async Task<Result> VerifyOtpAsync(
        Guid sessionId, 
        string otpCode, 
        CancellationToken cancellationToken = default)
    {
        var session = await _context.OtpSessions
            .SingleOrDefaultAsync(s => s.Id == sessionId, cancellationToken);

        if (session == null || session.Verified)
        {
            return Result.Failure(DoctorErrors.OtpSessionNotFound);
        }

        if (_dateTimeProvider.UtcNow > session.ExpiresAt)
        {
            return Result.Failure(DoctorErrors.OtpExpired);
        }

        if (session.Attempts >= 5)
        {
            return Result.Failure(DoctorErrors.OtpMaxAttemptsExceeded);
        }

        bool isValid = BCrypt.Net.BCrypt.Verify(otpCode.Trim(), session.OtpHash);

        if (!isValid)
        {
            session.Attempts++;
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Failure(DoctorErrors.OtpInvalid);
        }

        session.Verified = true;
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
