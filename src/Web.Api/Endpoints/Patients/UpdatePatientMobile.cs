using System;
using Application.Abstractions.Messaging;
using Application.Patients.UpdateMobile;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Patients;

internal sealed class UpdatePatientMobile : IEndpoint
{
    public sealed record Request(string MobileNumber);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/patients/{id:guid}/mobile", async (
            Guid id,
            Request request,
            ICommandHandler<UpdatePatientMobileCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdatePatientMobileCommand(id, request.MobileNumber);
            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Patients);
    }
}
