using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Patients.Otp;

public sealed record VerifyPatientOtpResponse(
    bool Verified,
    string? VerificationToken,
    string? ErrorMessage);

public sealed record VerifyPatientOtpCommand(
    Guid SessionId,
    string OtpCode) : ICommand<VerifyPatientOtpResponse>;

internal sealed class VerifyPatientOtpCommandHandler(
    IApplicationDbContext dbContext,
    IPasswordHasher passwordHasher,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<VerifyPatientOtpCommand, VerifyPatientOtpResponse>
{
    private const int MaxAttempts = 5;

    public async Task<Result<VerifyPatientOtpResponse>> Handle(VerifyPatientOtpCommand request, CancellationToken cancellationToken)
    {
        var session = await dbContext.PatientOtpSessions
            .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

        if (session == null)
        {
            return Result.Success(new VerifyPatientOtpResponse(
                Verified: false,
                VerificationToken: null,
                ErrorMessage: "OTP session not found or has expired. Please request a new code."));
        }

        if (session.Verified && !string.IsNullOrEmpty(session.VerificationToken))
        {
            return Result.Success(new VerifyPatientOtpResponse(
                Verified: true,
                VerificationToken: session.VerificationToken,
                ErrorMessage: null));
        }

        DateTime now = dateTimeProvider.UtcNow;

        if (now > session.ExpiresAt)
        {
            return Result.Success(new VerifyPatientOtpResponse(
                Verified: false,
                VerificationToken: null,
                ErrorMessage: "OTP code has expired. Please request a new verification code."));
        }

        if (session.Attempts >= MaxAttempts)
        {
            return Result.Success(new VerifyPatientOtpResponse(
                Verified: false,
                VerificationToken: null,
                ErrorMessage: "Maximum verification attempts exceeded. Please request a new code."));
        }

        bool isValid = passwordHasher.Verify(request.OtpCode.Trim(), session.OtpHash);

        if (!isValid)
        {
            session.Attempts++;
            await dbContext.SaveChangesAsync(cancellationToken);

            int remaining = MaxAttempts - session.Attempts;
            string error;
            if (remaining > 0)
            {
                string attemptText = remaining == 1 ? "attempt" : "attempts";
                error = $"Invalid verification code. {remaining} {attemptText} remaining.";
            }
            else
            {
                error = "Invalid verification code. Maximum attempts reached. Please request a new code.";
            }

            return Result.Success(new VerifyPatientOtpResponse(
                Verified: false,
                VerificationToken: null,
                ErrorMessage: error));
        }

        string token = Guid.NewGuid().ToString("N");
        session.Verified = true;
        session.VerificationToken = token;
        session.VerifiedAt = now;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new VerifyPatientOtpResponse(
            Verified: true,
            VerificationToken: token,
            ErrorMessage: null));
    }
}
