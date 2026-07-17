using Application.Abstractions.Messaging;
using Application.PracticeCentres.Commands;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using System.Security.Claims;

namespace Web.Api.Endpoints.PracticeCentres;

internal sealed class Create : IEndpoint
{
    public sealed record Request(
        string ClinicName,
        Guid PlaceId,
        int? MaxPatients,
        List<SessionGroupDto> SessionGroups,
        List<NurseDto> Nurses);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/practice-centres", async (
            Request request,
            ClaimsPrincipal user,
            ICommandHandler<CreatePracticeCentreCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var doctorIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (doctorIdClaim == null || !Guid.TryParse(doctorIdClaim, out var doctorId))
            {
                return Results.Unauthorized();
            }

            var command = new CreatePracticeCentreCommand(
                doctorId,
                request.ClinicName,
                request.PlaceId,
                request.MaxPatients,
                request.SessionGroups,
                request.Nurses);

            Result<Guid> result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.PracticeCentres)
        .RequireAuthorization();
    }
}
