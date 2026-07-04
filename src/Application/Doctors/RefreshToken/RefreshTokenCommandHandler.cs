using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Doctors.VerifyOtp;
using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Doctors.RefreshToken;

internal sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, TokenResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ITokenProvider _tokenProvider;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RefreshTokenCommandHandler(
        IApplicationDbContext context,
        ITokenProvider tokenProvider,
        IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _tokenProvider = tokenProvider;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<TokenResponse>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        // 1. Fetch account with matching refresh token
        DoctorAccount? account = await _context.DoctorAccounts
            .Include(a => a.Profile)
            .SingleOrDefaultAsync(a => a.RefreshToken == command.RefreshToken, cancellationToken);

        if (account == null)
        {
            return Result.Failure<TokenResponse>(DoctorErrors.Unauthorized);
        }

        // 2. Validate token expiry
        if (account.RefreshTokenExpiry == null || _dateTimeProvider.UtcNow > account.RefreshTokenExpiry)
        {
            // Invalidate the expired token
            account.RefreshToken = null;
            account.RefreshTokenExpiry = null;
            await _context.SaveChangesAsync(cancellationToken);
            
            return Result.Failure<TokenResponse>(DoctorErrors.Unauthorized);
        }

        // 3. Issue new tokens (rotation)
        string newAccessToken = _tokenProvider.Create(account);
        string newRefreshToken = _tokenProvider.CreateRefreshToken();

        // 4. Update account with new refresh token
        account.RefreshToken = newRefreshToken;
        account.RefreshTokenExpiry = _dateTimeProvider.UtcNow.AddDays(7); // Rotate and extend by 7 days

        await _context.SaveChangesAsync(cancellationToken);

        string completionStatus = account.Profile?.CompletionStatus.ToString() ?? nameof(ProfileCompletionStatus.MINIMAL);
        string fullName = account.Profile?.FullName ?? string.Empty;

        return new TokenResponse(
            newAccessToken,
            newRefreshToken,
            completionStatus,
            account.Id,
            account.Email,
            fullName
        );
    }
}
