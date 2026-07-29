using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Doctors.VerifyOtp;
using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Doctors.Login;

internal sealed class LoginDoctorCommandHandler : ICommandHandler<LoginDoctorCommand, TokenResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenProvider _tokenProvider;
    private readonly IDateTimeProvider _dateTimeProvider;

    public LoginDoctorCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        ITokenProvider tokenProvider,
        IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenProvider = tokenProvider;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<TokenResponse>> Handle(LoginDoctorCommand command, CancellationToken cancellationToken)
    {
        string emailNormalized = command.Email.ToLowerInvariant().Trim();

        // 1. Fetch DoctorAccount with Profile
        DoctorAccount? account = await _context.DoctorAccounts
            .Include(a => a.Profile)
            .SingleOrDefaultAsync(a => a.Email == emailNormalized, cancellationToken);

        if (account == null)
        {
            return Result.Failure<TokenResponse>(DoctorErrors.InvalidCredentials);
        }

        // If local authentication password hash is missing (e.g. registered with Google)
        if (account.AuthProvider == AuthProvider.GOOGLE || string.IsNullOrEmpty(account.PasswordHash))
        {
            return Result.Failure<TokenResponse>(DoctorErrors.InvalidCredentials);
        }

        // 2. Verify Password using BCrypt
        if (!_passwordHasher.Verify(command.Password, account.PasswordHash))
        {
            return Result.Failure<TokenResponse>(DoctorErrors.InvalidCredentials);
        }

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
        account.RefreshTokenExpiry = _dateTimeProvider.UtcNow.AddDays(7);

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

