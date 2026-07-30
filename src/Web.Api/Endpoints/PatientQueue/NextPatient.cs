using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.PatientQueue.Commands;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Endpoints.PatientQueue;

internal sealed class NextPatient : IEndpoint
{
    public sealed record Request(
        Guid DoctorId,
        Guid? PracticeCentreId = null,
        DateTime? VisitDate = null);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/queue/next-patient", HandleNextPatient)
            .WithTags(Tags.PatientQueue);

        app.MapPost("api/patient-queue/next-patient", HandleNextPatient)
            .WithTags(Tags.PatientQueue);
    }

    private static async Task<IResult> HandleNextPatient(
        [FromBody] Request request,
        ICommandHandler<AdvanceNextPatientCommand, NextPatientResponse> handler,
        CancellationToken cancellationToken)
    {
        var command = new AdvanceNextPatientCommand(
            request.DoctorId,
            request.PracticeCentreId,
            request.VisitDate);

        Result<NextPatientResponse> result = await handler.Handle(command, cancellationToken);
        return result.Match(Results.Ok, CustomResults.Problem);
    }
}
