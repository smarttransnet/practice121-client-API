using Domain.Doctors;
using SharedKernel;

namespace Application.Abstractions.Authentication;

public interface IOtpService
{
    Task<Guid> GenerateAndSendOtpAsync(Guid accountId, string destination, OtpChannel channel, CancellationToken cancellationToken = default);
    Task<Result> VerifyOtpAsync(Guid sessionId, string otpCode, CancellationToken cancellationToken = default);
}
