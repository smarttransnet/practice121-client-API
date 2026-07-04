using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Doctors.VerifyOtp;

internal sealed class VerifyOtpCommandHandler : ICommandHandler<VerifyOtpCommand, TokenResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IOtpService _otpService;
    private readonly ITokenProvider _tokenProvider;
    private readonly IDateTimeProvider _dateTimeProvider;

    public VerifyOtpCommandHandler(
        IApplicationDbContext context,
        IOtpService otpService,
        ITokenProvider tokenProvider,
        IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _otpService = otpService;
        _tokenProvider = tokenProvider;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<TokenResponse>> Handle(VerifyOtpCommand command, CancellationToken cancellationToken)
    {
        // 1. Verify OTP with service
        Result verifyResult = await _otpService.VerifyOtpAsync(command.SessionId, command.Code, cancellationToken);
        if (verifyResult.IsFailure)
        {
            return Result.Failure<TokenResponse>(verifyResult.Error);
        }

        // 2. Fetch OtpSession with Account and Profile
        var session = await _context.OtpSessions
            .Include(s => s.Account)
            .ThenInclude(a => a!.Profile)
            .SingleOrDefaultAsync(s => s.Id == command.SessionId, cancellationToken);

        if (session == null || session.Account == null)
        {
            return Result.Failure<TokenResponse>(DoctorErrors.OtpSessionNotFound);
        }

        DoctorAccount account = session.Account;

        // 3. Set Account status to ACTIVE if PENDING
        if (account.Status == AccountStatus.PENDING)
        {
            account.Status = AccountStatus.ACTIVE;
        }

        // 4. Generate JWT tokens
        string accessToken = _tokenProvider.Create(account);
        string refreshToken = _tokenProvider.CreateRefreshToken();

        // 5. Store Refresh Token
        account.RefreshToken = refreshToken;
        account.RefreshTokenExpiry = _dateTimeProvider.UtcNow.AddDays(7); // 7 days as requested

        await _context.SaveChangesAsync(cancellationToken);

        // 6. Return response
        string completionStatus = account.Profile?.CompletionStatus.ToString() ?? nameof(ProfileCompletionStatus.MINIMAL);
        string fullName = account.Profile?.FullName ?? string.Empty;

        return new TokenResponse(
            accessToken,
            refreshToken,
            completionStatus,
            account.Id,
            account.Email,
            fullName
        );
    }
}
