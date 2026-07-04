using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Doctors.Login;

internal sealed class LoginDoctorCommandHandler : ICommandHandler<LoginDoctorCommand, LoginDoctorResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IOtpService _otpService;

    public LoginDoctorCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IOtpService otpService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _otpService = otpService;
    }

    public async Task<Result<LoginDoctorResult>> Handle(LoginDoctorCommand command, CancellationToken cancellationToken)
    {
        string emailNormalized = command.Email.ToLowerInvariant().Trim();

        // 1. Fetch DoctorAccount
        DoctorAccount? account = await _context.DoctorAccounts
            .SingleOrDefaultAsync(a => a.Email == emailNormalized, cancellationToken);

        if (account == null)
        {
            return Result.Failure<LoginDoctorResult>(DoctorErrors.InvalidCredentials);
        }

        // If local authentication password hash is missing (e.g. registered with Google)
        if (account.AuthProvider == AuthProvider.GOOGLE || string.IsNullOrEmpty(account.PasswordHash))
        {
            return Result.Failure<LoginDoctorResult>(DoctorErrors.InvalidCredentials);
        }

        // 2. Verify Password using BCrypt
        if (!_passwordHasher.Verify(command.Password, account.PasswordHash))
        {
            return Result.Failure<LoginDoctorResult>(DoctorErrors.InvalidCredentials);
        }

        // 3. Trigger OTP MFA to registered email
        Guid otpSessionId = await _otpService.GenerateAndSendOtpAsync(
            account.Id, 
            account.Email, 
            OtpChannel.EMAIL, 
            cancellationToken);

        return new LoginDoctorResult(account.Id, otpSessionId);
    }
}
