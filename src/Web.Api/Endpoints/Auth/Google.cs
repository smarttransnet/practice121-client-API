using Application.Abstractions.Messaging;
using Application.Doctors.GoogleAuth;
using SharedKernel;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Auth;

internal sealed class Google : IEndpoint
{
    public sealed record Request(string IdToken);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/auth/google", async (
            Request request,
            ICommandHandler<GoogleAuthCommand, GoogleAuthResult> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new GoogleAuthCommand(request.IdToken);

            Result<GoogleAuthResult> result = await handler.Handle(command, cancellationToken);

            return result.ToApiResponse();
        })
        .WithTags(Tags.Auth);
    }
}
