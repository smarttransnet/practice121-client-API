using Application.Abstractions.Messaging;
using Application.Doctors.Register;
using SharedKernel;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Auth;

internal sealed class Register : IEndpoint
{
    public sealed record Request(string Name, string Email, string Password);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/auth/register", async (
            Request request,
            ICommandHandler<RegisterDoctorCommand, RegisterDoctorResult> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RegisterDoctorCommand(request.Name, request.Email, request.Password);

            Result<RegisterDoctorResult> result = await handler.Handle(command, cancellationToken);

            return result.ToApiResponse();
        })
        .WithTags(Tags.Auth);
    }
}
