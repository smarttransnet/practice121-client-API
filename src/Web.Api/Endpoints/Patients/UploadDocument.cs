using Application.Abstractions.Messaging;
using Application.Patients.UploadDocument;
using Domain.Doctors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Patients;

internal sealed class UploadDocument : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/patients/{id:guid}/documents", async (
            Guid id,
            [FromForm] DocumentType type,
            IFormFile file,
            ICommandHandler<UploadPatientDocumentCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            if (file == null || file.Length == 0)
            {
                return Results.BadRequest("No file uploaded.");
            }

            using var stream = file.OpenReadStream();
            var command = new UploadPatientDocumentCommand(id, type, stream, file.ContentType);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Patients)
        .DisableAntiforgery();
    }
}
