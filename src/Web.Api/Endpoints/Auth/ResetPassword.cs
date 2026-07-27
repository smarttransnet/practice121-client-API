using Application.Abstractions.Messaging;
using Application.Doctors.ResetPassword;
using SharedKernel;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Auth;

internal sealed class ResetPassword : IEndpoint
{
    public sealed record Request(
        string AccountId,
        string Token,
        string NewPassword,
        string ConfirmPassword);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/auth/reset-password", async (
            Request request,
            ICommandHandler<ResetPasswordCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ResetPasswordCommand(
                request.AccountId,
                request.Token,
                request.NewPassword,
                request.ConfirmPassword);

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.ToApiResponse();
        })
        .WithTags(Tags.Auth)
        .AllowAnonymous();
    }
}
