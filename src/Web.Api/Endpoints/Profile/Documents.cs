using Application.Abstractions.Messaging;
using Application.Doctors.UploadDocument;
using Domain.Doctors;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Profile;

internal sealed class Documents : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/profile/documents", async (
            IFormFile file,
            [FromForm] int type,
            ICommandHandler<UploadDocumentCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            if (file == null || file.Length == 0)
            {
                return Results.BadRequest(ApiResponse<object>.CreateFailure("Documents.InvalidFile", "No file uploaded or file is empty"));
            }

            using Stream stream = file.OpenReadStream();
            var command = new UploadDocumentCommand((DocumentType)type, stream, file.FileName, file.ContentType);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.ToApiResponse();
        })
        .DisableAntiforgery() // Disable antiforgery token verification for API convenience
        .RequireAuthorization()
        .WithTags(Tags.Profile);
    }
}
