using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Doctors.GoogleAuth;

internal sealed class GoogleAuthCommandHandler : ICommandHandler<GoogleAuthCommand, GoogleAuthResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IGoogleAuthService _googleAuthService;
    private readonly IOtpService _otpService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GoogleAuthCommandHandler(
        IApplicationDbContext context,
        IGoogleAuthService googleAuthService,
        IOtpService otpService,
        IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _googleAuthService = googleAuthService;
        _otpService = otpService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<GoogleAuthResult>> Handle(GoogleAuthCommand command, CancellationToken cancellationToken)
    {
        // 1. Verify token with Google
        GoogleUserResult? googleUser = await _googleAuthService.VerifyTokenAsync(command.IdToken, cancellationToken);
        if (googleUser == null)
        {
            return Result.Failure<GoogleAuthResult>(DoctorErrors.InvalidGoogleToken);
        }

        string emailNormalized = googleUser.Email.ToLowerInvariant().Trim();

        // 2. Query DoctorAccounts by GoogleSubId
        DoctorAccount? account = await _context.DoctorAccounts
            .SingleOrDefaultAsync(a => a.GoogleSubId == googleUser.Sub, cancellationToken);

        if (account == null)
        {
            // If sub not found, check if email already registered (maybe via manual register)
            if (await _context.DoctorAccounts.AnyAsync(a => a.Email == emailNormalized, cancellationToken))
            {
                return Result.Failure<GoogleAuthResult>(DoctorErrors.EmailNotUnique);
            }

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

        // 3. Generate & Send OTP for MFA
        Guid otpSessionId = await _otpService.GenerateAndSendOtpAsync(
            account.Id, 
            account.Email, 
            OtpChannel.EMAIL, 
            cancellationToken);

        return new GoogleAuthResult(account.Id, otpSessionId);
    }
}
