using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.PatientQueue.Queries;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Endpoints.PatientQueue;

internal sealed class GetList : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/patient-queue", async (
            [FromQuery] Guid practiceCentreId,
            [FromQuery] Guid? doctorId,
            [FromQuery] DateTime? visitDate,
            IQueryHandler<GetPatientQueueListQuery, List<PatientQueueTicketResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPatientQueueListQuery(practiceCentreId, doctorId, visitDate);

            Result<List<PatientQueueTicketResponse>> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.PatientQueue)
        .RequireAuthorization();
    }
}
