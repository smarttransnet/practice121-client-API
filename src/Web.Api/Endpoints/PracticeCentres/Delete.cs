using Application.Abstractions.Messaging;
using Application.PracticeCentres.Commands;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using System.Security.Claims;

namespace Web.Api.Endpoints.PracticeCentres;

internal sealed class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/practice-centres/{id:guid}", async (
            Guid id,
            ClaimsPrincipal user,
            ICommandHandler<DeletePracticeCentreCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var doctorIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (doctorIdClaim == null || !Guid.TryParse(doctorIdClaim, out var doctorId))
            {
                return Results.Unauthorized();
            }

            var command = new DeletePracticeCentreCommand(id, doctorId);
            Result result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.PracticeCentres)
        .RequireAuthorization();
    }
}
