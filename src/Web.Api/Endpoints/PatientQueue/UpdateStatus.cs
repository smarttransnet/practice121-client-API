using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.PatientQueue.Commands;
using Domain.PatientQueue;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Endpoints.PatientQueue;

internal sealed class UpdateStatus : IEndpoint
{
    public sealed record Request(PatientQueueStatus Status);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/patient-queue/{id:guid}/status", async (
            Guid id,
            [FromBody] Request request,
            ICommandHandler<UpdatePatientQueueTicketStatusCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdatePatientQueueTicketStatusCommand(id, request.Status);

            Result result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.PatientQueue)
        .RequireAuthorization();
    }
}
