using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.SMS;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Patients.Otp;

public sealed record ResendPatientOtpResponse(
    bool Success,
    string? ErrorMessage,
    int CooldownSeconds,
    int ExpiresInSeconds);

public sealed record ResendPatientOtpCommand(Guid SessionId) : ICommand<ResendPatientOtpResponse>;

internal sealed class ResendPatientOtpCommandHandler(
    IApplicationDbContext dbContext,
    ISmsService smsService,
    ISmsTemplateService smsTemplateService,
    IPasswordHasher passwordHasher,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<ResendPatientOtpCommand, ResendPatientOtpResponse>
{
    private const int CooldownDurationSeconds = 60;

    public async Task<Result<ResendPatientOtpResponse>> Handle(ResendPatientOtpCommand request, CancellationToken cancellationToken)
    {
        var session = await dbContext.PatientOtpSessions
            .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

        if (session == null || session.Verified)
        {
            return Result.Success(new ResendPatientOtpResponse(
                Success: false,
                ErrorMessage: "OTP session not found or already verified.",
                CooldownSeconds: 0,
                ExpiresInSeconds: 0));
        }

        DateTime now = dateTimeProvider.UtcNow;
        TimeSpan timeSinceLastSent = now - session.LastSentAt;

        if (timeSinceLastSent.TotalSeconds < CooldownDurationSeconds)
        {
            int remainingCooldown = CooldownDurationSeconds - (int)timeSinceLastSent.TotalSeconds;
            return Result.Success(new ResendPatientOtpResponse(
                Success: false,
                ErrorMessage: $"Please wait {remainingCooldown} seconds before requesting a new code.",
                CooldownSeconds: remainingCooldown,
                ExpiresInSeconds: (int)(session.ExpiresAt - now).TotalSeconds));
        }

        int code = RandomNumberGenerator.GetInt32(100000, 1000000);
        string otpCode = code.ToString(System.Globalization.CultureInfo.InvariantCulture);
        string otpHash = passwordHasher.Hash(otpCode);

        session.OtpHash = otpHash;
        session.ExpiresAt = now.AddMinutes(10);
        session.LastSentAt = now;
        session.Attempts = 0;

        await dbContext.SaveChangesAsync(cancellationToken);

        string smsMessage = smsTemplateService.RenderTemplate(
            SmsTemplateType.OtpVerification,
            new Dictionary<string, string>
            {
                { "OTP", otpCode },
                { "ApplicationName", "Practice121" }
            });

        _ = Task.Run(async () =>
        {
            try
            {
                await smsService.SendSmsAsync(session.MobileNumber, smsMessage, CancellationToken.None);
            }
            catch
            {
                // Error logged in SmsService
            }
        }, CancellationToken.None);

        return Result.Success(new ResendPatientOtpResponse(
            Success: true,
            ErrorMessage: null,
            CooldownSeconds: CooldownDurationSeconds,
            ExpiresInSeconds: 600));
    }
}
