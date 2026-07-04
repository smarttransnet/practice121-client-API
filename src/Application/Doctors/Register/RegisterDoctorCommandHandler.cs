using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Doctors.Register;

internal sealed class RegisterDoctorCommandHandler : ICommandHandler<RegisterDoctorCommand, RegisterDoctorResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IOtpService _otpService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RegisterDoctorCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IOtpService otpService,
        IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _otpService = otpService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<RegisterDoctorResult>> Handle(RegisterDoctorCommand command, CancellationToken cancellationToken)
    {
        string emailNormalized = command.Email.ToLowerInvariant().Trim();

        // 1. Validate email uniqueness
        if (await _context.DoctorAccounts.AnyAsync(a => a.Email == emailNormalized, cancellationToken))
        {
            return Result.Failure<RegisterDoctorResult>(DoctorErrors.EmailNotUnique);
        }

        // 2. Create DoctorAccount
        var account = new DoctorAccount
        {
            Id = Guid.NewGuid(),
            AuthProvider = AuthProvider.LOCAL,
            Email = emailNormalized,
            PasswordHash = _passwordHasher.Hash(command.Password),
            GoogleSubId = null,
            Status = AccountStatus.PENDING,
            CreatedAt = _dateTimeProvider.UtcNow
        };

        // 3. Create Blank DoctorProfile
        var nameParts = command.Name.Trim().Split(' ', 2);
        string firstName = nameParts[0];
        string lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

        var profile = new DoctorProfile
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            FullName = command.Name,
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

        // 4. Generate & Send OTP
        Guid otpSessionId = await _otpService.GenerateAndSendOtpAsync(
            account.Id, 
            account.Email, 
            OtpChannel.EMAIL, 
            cancellationToken);

        return new RegisterDoctorResult(account.Id, otpSessionId);
    }
}
