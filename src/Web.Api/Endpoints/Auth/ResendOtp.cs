using Application.Abstractions.Messaging;
using Application.Doctors.ResendOtp;
using Domain.Doctors;
using SharedKernel;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Auth;

internal sealed class ResendOtp : IEndpoint
{
    public sealed record Request(Guid AccountId, int Channel);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/auth/resend-otp", async (
            Request request,
            ICommandHandler<ResendOtpCommand, ResendOtpResult> handler,
            CancellationToken cancellationToken) =>
        {
            var otpChannel = (OtpChannel)request.Channel;
            var command = new ResendOtpCommand(request.AccountId, otpChannel);

            Result<ResendOtpResult> result = await handler.Handle(command, cancellationToken);

            return result.ToApiResponse();
        })
        .WithTags(Tags.Auth);
    }
}
