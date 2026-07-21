using Application.Doctors.GetPublicDoctor;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Web.Api.Endpoints.Public;

public class GetPublicDoctor : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/public/doctors/{accountId}", async (
            Guid accountId,
            [Microsoft.AspNetCore.Mvc.FromServices] Application.Abstractions.Messaging.IQueryHandler<GetPublicDoctorQuery, PublicDoctorResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPublicDoctorQuery(accountId);
            var result = await handler.Handle(query, cancellationToken);

            return result.IsSuccess ? Results.Ok(result.Value) : Web.Api.Infrastructure.CustomResults.Problem(result);
        })
        .WithTags("Public")
        .AllowAnonymous();
    }
}
