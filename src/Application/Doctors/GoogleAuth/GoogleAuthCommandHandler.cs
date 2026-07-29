using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Doctors.VerifyOtp;
using Application.Abstractions.Messaging;
using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Doctors.GoogleAuth;

internal sealed class GoogleAuthCommandHandler : ICommandHandler<GoogleAuthCommand, TokenResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IGoogleAuthService _googleAuthService;
    private readonly ITokenProvider _tokenProvider;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GoogleAuthCommandHandler(
        IApplicationDbContext context,
        IGoogleAuthService googleAuthService,
        ITokenProvider tokenProvider,
        IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _googleAuthService = googleAuthService;
        _tokenProvider = tokenProvider;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<TokenResponse>> Handle(GoogleAuthCommand command, CancellationToken cancellationToken)
    {
        // 1. Verify token with Google
        GoogleUserResult? googleUser = await _googleAuthService.VerifyTokenAsync(command.IdToken, cancellationToken);
        if (googleUser == null)
        {
            return Result.Failure<TokenResponse>(DoctorErrors.InvalidGoogleToken);
        }

        string emailNormalized = googleUser.Email.ToLowerInvariant().Trim();

        // 2. Query DoctorAccounts by GoogleSubId
        DoctorAccount? account = await _context.DoctorAccounts
            .Include(a => a.Profile)
            .SingleOrDefaultAsync(a => a.GoogleSubId == googleUser.Sub, cancellationToken);

        if (account == null)
        {
            // If sub not found, check if email already registered (e.g. via manual registration)
            account = await _context.DoctorAccounts
                .Include(a => a.Profile)
                .SingleOrDefaultAsync(a => a.Email == emailNormalized, cancellationToken);

            if (account != null)
            {
                // Link GoogleSubId to existing account
                account.GoogleSubId = googleUser.Sub;
                await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                // Create new DoctorAccount
                account = new DoctorAccount
                {
                    Id = Guid.NewGuid(),
                    AuthProvider = AuthProvider.GOOGLE,
                    Email = emailNormalized,
                    PasswordHash = null,
                    GoogleSubId = googleUser.Sub,
                    Status = AccountStatus.PENDING,
                    CreatedAt = _dateTimeProvider.UtcNow
                };

            // Create Blank DoctorProfile
            var nameParts = googleUser.Name.Trim().Split(' ', 2);
            string firstName = nameParts[0];
            string lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

            var profile = new DoctorProfile
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                FullName = googleUser.Name,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = null,
                SlmcRegNumber = null,
                NicNumber = null,
                MobileNumber = null,
                Qualifications = [],
                Specialty = null,
                ProfilePictureUrl = null,
                CompletionStatus = ProfileCompletionStatus.MINIMAL
            };

            _context.DoctorAccounts.Add(account);
            _context.DoctorProfiles.Add(profile);

            await _context.SaveChangesAsync(cancellationToken);
            }
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
