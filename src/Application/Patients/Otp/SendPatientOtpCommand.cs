using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.SMS;
using Domain.Patients;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Patients.Otp;

public sealed record SendPatientOtpResponse(
    bool PatientExists,
    Guid? SessionId,
    string? MaskedMobile,
    int ExpiresInSeconds,
    int CooldownSeconds);

public sealed record SendPatientOtpCommand(string MobileNumber) : ICommand<SendPatientOtpResponse>;

internal sealed class SendPatientOtpCommandHandler(
    IApplicationDbContext dbContext,
    ISmsService smsService,
    ISmsTemplateService smsTemplateService,
    IPasswordHasher passwordHasher,
    IDateTimeProvider dateTimeProvider,
    FeatureFlags featureFlags,
    ILogger<SendPatientOtpCommandHandler> logger)
    : ICommandHandler<SendPatientOtpCommand, SendPatientOtpResponse>
{
    public async Task<Result<SendPatientOtpResponse>> Handle(SendPatientOtpCommand request, CancellationToken cancellationToken)
    {
        string rawMobile = request.MobileNumber.Trim();
        string normalizedMobile = SriLankanPhoneValidator.NormalizeToE164(rawMobile) ?? rawMobile;

        bool patientExists = await dbContext.PatientAccounts
            .AsNoTracking()
            .AnyAsync(p => p.MobileNumber == normalizedMobile || p.MobileNumber == rawMobile, cancellationToken);

        int code = RandomNumberGenerator.GetInt32(100000, 1000000);
        string otpCode = code.ToString(System.Globalization.CultureInfo.InvariantCulture);

        string otpHash = passwordHasher.Hash(otpCode);

        DateTime now = dateTimeProvider.UtcNow;
        DateTime expiresAt = now.AddMinutes(10);

        var session = new PatientOtpSession
        {
            Id = Guid.NewGuid(),
            MobileNumber = normalizedMobile,
            OtpHash = otpHash,
            ExpiresAt = expiresAt,
            Verified = false,
            VerificationToken = null,
            VerifiedAt = null,
            Attempts = 0,
            CreatedAt = now,
            LastSentAt = now
        };

        dbContext.PatientOtpSessions.Add(session);
        await dbContext.SaveChangesAsync(cancellationToken);

        bool smsOtpEnabled = featureFlags.SmsOtpEnabled;
        if (smsOtpEnabled)
        {
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
                    await smsService.SendSmsAsync(normalizedMobile, smsMessage, CancellationToken.None);
                }
                catch
                {
                    // Error logged in SmsService
                }
            }, CancellationToken.None);
        }
        else
        {
            logger.LogInformation(
                "[SMS OTP DISABLED] Skipped sending patient SMS OTP to {MobileNumber}. " +
                "Any 6-digit numeric code will be accepted during verification. " +
                "Set Features:SmsOtpEnabled=true to re-enable real SMS delivery.",
                normalizedMobile);
        }

        string maskedMobile = MaskMobileNumber(rawMobile);

        return Result.Success(new SendPatientOtpResponse(
            PatientExists: patientExists,
            SessionId: session.Id,
            MaskedMobile: maskedMobile,
            ExpiresInSeconds: 600,
            CooldownSeconds: 60));
    }

    private static string MaskMobileNumber(string mobile)
    {
        string digitsOnly = System.Text.RegularExpressions.Regex.Replace(mobile, @"[^\d]", "");
        if (digitsOnly.Length < 7)
        {
            return mobile;
        }

        return string.Concat(digitsOnly.AsSpan(0, 3), "****", digitsOnly.AsSpan(digitsOnly.Length - 3));
    }
}
