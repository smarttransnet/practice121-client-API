using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Patients.AddChild;
using Application.Patients.GetByMobile;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Patients;

internal sealed class AddChild : IEndpoint
{
    public sealed record Request(
        string FullName,
        DateTime DateOfBirth,
        string Gender);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/patients/{parentId:guid}/children", async (
            Guid parentId,
            Request request,
            ICommandHandler<AddChildPatientCommand, PatientResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AddChildPatientCommand(
                parentId,
                request.FullName,
                request.DateOfBirth,
                request.Gender);

            Result<PatientResponse> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                response => Results.Created($"api/patients/{response.Id}", response),
                CustomResults.Problem);
        })
        .WithTags(Tags.Patients);
    }
}
