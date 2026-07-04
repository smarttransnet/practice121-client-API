using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Doctors.ResendOtp;

internal sealed class ResendOtpCommandHandler : ICommandHandler<ResendOtpCommand, ResendOtpResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IOtpService _otpService;

    public ResendOtpCommandHandler(
        IApplicationDbContext context,
        IOtpService otpService)
    {
        _context = context;
        _otpService = otpService;
    }

    public async Task<Result<ResendOtpResult>> Handle(ResendOtpCommand command, CancellationToken cancellationToken)
    {
        // 1. Fetch account
        DoctorAccount? account = await _context.DoctorAccounts
            .SingleOrDefaultAsync(a => a.Id == command.AccountId, cancellationToken);

        if (account == null)
        {
            return Result.Failure<ResendOtpResult>(DoctorErrors.NotFound(command.AccountId));
        }

        // 2. Generate and send a new OTP session
        // Note: For now, we simulate destination using the account's registered email.
        Guid otpSessionId = await _otpService.GenerateAndSendOtpAsync(
            account.Id, 
            account.Email, 
            command.Channel, 
            cancellationToken);

        return new ResendOtpResult(otpSessionId);
    }
}
