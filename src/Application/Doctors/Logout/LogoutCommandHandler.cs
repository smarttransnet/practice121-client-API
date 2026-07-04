using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Doctors.Logout;

internal sealed class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
    private readonly IApplicationDbContext _context;

    public LogoutCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        // 1. Fetch account with matching refresh token
        DoctorAccount? account = await _context.DoctorAccounts
            .SingleOrDefaultAsync(a => a.RefreshToken == command.RefreshToken, cancellationToken);

        if (account != null)
        {
            // 2. Clear refresh token details
            account.RefreshToken = null;
            account.RefreshTokenExpiry = null;

            await _context.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
