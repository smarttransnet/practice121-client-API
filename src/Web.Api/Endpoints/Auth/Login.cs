using Application.Abstractions.Messaging;
using Application.Doctors.Login;
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
            ICommandHandler<LoginDoctorCommand, LoginDoctorResult> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new LoginDoctorCommand(request.Email, request.Password);

            Result<LoginDoctorResult> result = await handler.Handle(command, cancellationToken);

            return result.ToApiResponse();
        })
        .WithTags(Tags.Auth);
    }
}
