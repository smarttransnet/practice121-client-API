using Application.Abstractions.Messaging;
using Application.Patients.GetByMobile;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Patients;

internal sealed class GetPatientByMobile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/patients/by-mobile", async (
            string mobileNumber,
            IQueryHandler<GetPatientByMobileQuery, PatientResponse?> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPatientByMobileQuery(mobileNumber);
            Result<PatientResponse?> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                response => response is null ? Results.NotFound() : Results.Ok(response),
                CustomResults.Problem);
        })
        .WithTags(Tags.Patients);
    }
}
