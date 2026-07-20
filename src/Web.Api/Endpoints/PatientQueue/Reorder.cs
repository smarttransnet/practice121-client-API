using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.PatientQueue.Commands;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.PatientQueue;

internal sealed class Reorder : IEndpoint
{
    public sealed record Request(List<Guid> TicketIds);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/patient-queue/reorder", async (
            Request request,
            ICommandHandler<ReorderPatientQueueTicketsCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ReorderPatientQueueTicketsCommand(request.TicketIds);

            Result result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.PatientQueue)
        .RequireAuthorization();
    }
}
