using Application.Abstractions.Messaging;
using Application.PracticeCentres.Queries;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using System.Security.Claims;

namespace Web.Api.Endpoints.PracticeCentres;

internal sealed class Get : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/practice-centres", async (
            ClaimsPrincipal user,
            IQueryHandler<GetPracticeCentresQuery, List<PracticeCentreResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var doctorIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (doctorIdClaim == null || !Guid.TryParse(doctorIdClaim, out var doctorId))
            {
                return Results.Unauthorized();
            }

            var query = new GetPracticeCentresQuery(doctorId);
            Result<List<PracticeCentreResponse>> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.PracticeCentres)
        .RequireAuthorization();
    }
}
