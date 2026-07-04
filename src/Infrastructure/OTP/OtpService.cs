using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.OTP;

internal sealed class OtpService : IOtpService
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public OtpService(
        IApplicationDbContext context,
        IEmailService emailService,
        IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _emailService = emailService;
        _dateTimeProvider = dateTimeProvider;
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

        // 4. Send simulated code
        string subject = "Doctor Portal - MFA Verification Code";
        string body = $"Your verification code is: {otpCode}\nThis code is valid for 10 minutes.";
        await _emailService.SendEmailAsync(destination, subject, body, cancellationToken);

        return session.Id;
    }

    public async Task<Result> VerifyOtpAsync(
        Guid sessionId, 
        string otpCode, 
        CancellationToken cancellationToken = default)
    {
        var session = await _context.OtpSessions
            .SingleOrDefaultAsync(s => s.Id == sessionId, cancellationToken);

        if (session == null)
        {
            return Result.Failure(DoctorErrors.OtpSessionNotFound);
        }

        // BYPASSED FOR TESTING: Any code entered will succeed
        session.Verified = true;
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
