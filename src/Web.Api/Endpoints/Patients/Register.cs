using Application.Abstractions.Messaging;
using Application.Patients.Register;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Patients;

internal sealed class Register : IEndpoint
{
    public sealed record Request(
        string NicNumber,
        string FirstName,
        string? LastName,
        DateTime? DateOfBirth,
        string? Gender,
        string MobileNumber,
        Guid? CreatedByDoctorId);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/patients/register", async (
            Request request,
            ICommandHandler<RegisterPatientCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RegisterPatientCommand(
                request.NicNumber,
                request.FirstName,
                request.LastName,
                request.DateOfBirth,
                request.Gender,
                request.MobileNumber,
                request.CreatedByDoctorId);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Patients);
    }
}
