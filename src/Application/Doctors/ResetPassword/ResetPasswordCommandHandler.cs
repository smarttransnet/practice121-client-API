using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Doctors.ResetPassword;

internal sealed class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    public ResetPasswordCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IDateTimeProvider dateTimeProvider,
        ILogger<ResetPasswordCommandHandler> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<Result<string>> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        // Parse AccountId
        if (!Guid.TryParse(command.AccountId, out Guid accountId))
        {
            return Result.Failure<string>(DoctorErrors.InvalidOrExpiredResetToken);
        }

        // Fetch account
        DoctorAccount? account = await _context.DoctorAccounts
            .SingleOrDefaultAsync(a => a.Id == accountId, cancellationToken);

        if (account is null)
        {
            return Result.Failure<string>(DoctorErrors.InvalidOrExpiredResetToken);
        }

        // Validate token fields exist
        if (string.IsNullOrEmpty(account.PasswordResetTokenHash) || account.PasswordResetTokenExpiry is null)
        {
            return Result.Failure<string>(DoctorErrors.InvalidOrExpiredResetToken);
        }

        // Check expiry
        if (_dateTimeProvider.UtcNow > account.PasswordResetTokenExpiry.Value)
        {
            // Clean up expired token
            account.PasswordResetTokenHash = null;
            account.PasswordResetTokenExpiry = null;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogWarning("Expired reset token used for account {AccountId}.", accountId);
            return Result.Failure<string>(DoctorErrors.InvalidOrExpiredResetToken);
        }

        // Verify the raw token against the stored hash using IPasswordHasher
        bool tokenValid = _passwordHasher.Verify(command.Token, account.PasswordResetTokenHash);

        if (!tokenValid)
        {
            _logger.LogWarning("Invalid reset token supplied for account {AccountId}.", accountId);
            return Result.Failure<string>(DoctorErrors.InvalidOrExpiredResetToken);
        }

        // Hash and set new password using the existing IPasswordHasher (BCrypt WF=12)
        account.PasswordHash = _passwordHasher.Hash(command.NewPassword);

        // Invalidate token immediately — single-use
        account.PasswordResetTokenHash = null;
        account.PasswordResetTokenExpiry = null;

        // Also invalidate any existing refresh tokens to force re-login
        account.RefreshToken = null;
        account.RefreshTokenExpiry = null;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Password successfully reset for account {AccountId}.", accountId);

        return "Your password has been reset successfully. You can now log in with your new password.";
    }
}
