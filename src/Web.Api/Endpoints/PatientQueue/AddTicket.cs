using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.PatientQueue.Commands;
using Domain.PatientQueue;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.PatientQueue;

internal sealed class AddTicket : IEndpoint
{
    public sealed record Request(
        string PatientMobile,
        Guid DoctorId,
        Guid PracticeCentreId,
        PatientQueuePriority Priority,
        DateTime? VisitDate,
        Guid? PatientId = null);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/patient-queue", async (
            Request request,
            ICommandHandler<AddPatientQueueTicketCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AddPatientQueueTicketCommand(
                request.PatientMobile,
                request.DoctorId,
                request.PracticeCentreId,
                request.Priority,
                request.VisitDate,
                request.PatientId);

            Result<Guid> result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.PatientQueue)
        .RequireAuthorization();
    }
}
