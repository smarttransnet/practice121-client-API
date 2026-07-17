using Application.Abstractions.Messaging;
using Application.PracticeCentres.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Public;

public class GetDoctorPracticeCentres : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/public/doctors/{accountId}/practice-centres", async (
            Guid accountId,
            [Microsoft.AspNetCore.Mvc.FromServices] IQueryHandler<GetPracticeCentresQuery, List<PracticeCentreResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPracticeCentresQuery(accountId);
            var result = await handler.Handle(query, cancellationToken);
            
            return result.IsSuccess ? Results.Ok(result.Value) : CustomResults.Problem(result);
        })
        .WithTags("Public")
        .AllowAnonymous();
    }
}
