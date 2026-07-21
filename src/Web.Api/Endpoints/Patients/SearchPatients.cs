using Application.Abstractions.Messaging;
using Application.Patients.GetByMobile;
using Application.Patients.Search;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Patients;

internal sealed class SearchPatients : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/patients/search", async (
            string? firstName,
            string? lastName,
            string? nicNumber,
            IQueryHandler<SearchPatientsQuery, List<PatientResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new SearchPatientsQuery(firstName, lastName, nicNumber);
            Result<List<PatientResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Patients);
    }
}
