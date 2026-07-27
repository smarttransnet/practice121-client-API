using Application.Abstractions.Messaging;
using Application.Doctors.ForgotPassword;
using SharedKernel;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Auth;

internal sealed class ForgotPassword : IEndpoint
{
    public sealed record Request(string Email);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/auth/forgot-password", async (
            Request request,
            ICommandHandler<ForgotPasswordCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ForgotPasswordCommand(request.Email);

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.ToApiResponse();
        })
        .WithTags(Tags.Auth)
        .AllowAnonymous();
    }
}
