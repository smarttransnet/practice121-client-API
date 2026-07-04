using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Profile;

internal sealed class Avatar : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/profile/avatar", async (
            IFormFile file,
            IApplicationDbContext context,
            IUserContext userContext,
            CancellationToken cancellationToken) =>
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return Results.BadRequest(ApiResponse<object>.CreateFailure("Avatar.InvalidFile", "No file uploaded or file is empty"));
                }

                // Enforce size <= 2MB (2 * 1024 * 1024 bytes)
                if (file.Length > 2 * 1024 * 1024)
                {
                    return Results.BadRequest(ApiResponse<object>.CreateFailure("Avatar.FileTooLarge", "File size cannot exceed 2 MB"));
                }

                // Enforce type is image/jpeg or image/png
                string contentType = file.ContentType.ToLowerInvariant();
                if (contentType != "image/jpeg" && contentType != "image/png" && contentType != "image/jpg")
                {
                    return Results.BadRequest(ApiResponse<object>.CreateFailure("Avatar.InvalidType", "Only JPEG and PNG images are allowed"));
                }

                Guid accountId = userContext.UserId;
                var profile = await context.DoctorProfiles
                    .SingleOrDefaultAsync(p => p.AccountId == accountId, cancellationToken);

                if (profile == null)
                {
                    return Results.NotFound(ApiResponse<object>.CreateFailure("Avatar.ProfileNotFound", "Doctor profile not found"));
                }

                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream, cancellationToken);
                
                profile.ProfilePictureData = memoryStream.ToArray();
                profile.ProfilePictureContentType = file.ContentType;
                profile.ProfilePictureUrl = null; // Clear legacy url

                string fileUrl = $"/api/files/avatar/{accountId}";
                await context.SaveChangesAsync(cancellationToken);

                return Results.Ok(ApiResponse<string>.CreateSuccess(fileUrl));
            }
            catch (Exception ex)
            {
                return Results.Json(ApiResponse<object>.CreateFailure("Avatar.Exception", $"{ex.Message}\n{ex.StackTrace}"), statusCode: 500);
            }
        })
        .DisableAntiforgery()
        .RequireAuthorization()
        .WithTags(Tags.Profile);
    }
}
