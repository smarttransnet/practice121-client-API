using Application.Abstractions.Messaging;
using Application.Doctors.Login;
using Application.Doctors.VerifyOtp;
using SharedKernel;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Auth;

internal sealed class Login : IEndpoint
{
    public sealed record Request(string Email, string Password);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/auth/login", async (
            Request request,
            ICommandHandler<LoginDoctorCommand, TokenResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new LoginDoctorCommand(request.Email, request.Password);

            Result<TokenResponse> result = await handler.Handle(command, cancellationToken);

            return result.ToApiResponse();
        })
        .WithTags(Tags.Auth);
    }
}

