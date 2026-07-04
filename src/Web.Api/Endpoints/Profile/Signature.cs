using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Profile;

internal sealed class Signature : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/profile/signature", async (
            IFormFile file,
            HttpContext httpContext,
            IApplicationDbContext context,
            IUserContext userContext,
            IDateTimeProvider dateTimeProvider,
            CancellationToken cancellationToken) =>
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return Results.BadRequest(ApiResponse<object>.CreateFailure("Signature.InvalidFile", "No file uploaded or file is empty"));
                }

                // Enforce size <= 2MB (2 * 1024 * 1024 bytes)
                if (file.Length > 2 * 1024 * 1024)
                {
                    return Results.BadRequest(ApiResponse<object>.CreateFailure("Signature.FileTooLarge", "File size cannot exceed 2 MB"));
                }

                // Enforce type is image/jpeg, image/png, or image/svg+xml
                string contentType = file.ContentType.ToLowerInvariant();
                if (contentType != "image/jpeg" && contentType != "image/png" && contentType != "image/jpg" && contentType != "image/svg+xml")
                {
                    return Results.BadRequest(ApiResponse<object>.CreateFailure("Signature.InvalidType", "Only JPEG, PNG, and SVG images are allowed"));
                }

                Guid accountId = userContext.UserId;
                var profile = await context.DoctorProfiles
                    .Include(p => p.Documents)
                    .Include(p => p.ESignature)
                    .SingleOrDefaultAsync(p => p.AccountId == accountId, cancellationToken);

                if (profile == null)
                {
                    return Results.NotFound(ApiResponse<object>.CreateFailure("Signature.ProfileNotFound", "Doctor profile not found"));
                }

                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream, cancellationToken);
                byte[] fileBytes = memoryStream.ToArray();

                string relativePath = $"/api/files/signature/{accountId}";
                string ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

                // Create or Update ESignature
                if (profile.ESignature != null)
                {
                    profile.ESignature.SignatureDataUrl = string.Empty; // Legacy
                    profile.ESignature.SignatureData = fileBytes;
                    profile.ESignature.SignatureContentType = file.ContentType;
                    profile.ESignature.IsActive = true;
                    profile.ESignature.SignedAt = dateTimeProvider.UtcNow;
                    profile.ESignature.IpAddress = ipAddress;
                }
                else
                {
                    var signature = new ESignature
                    {
                        Id = Guid.NewGuid(),
                        ProfileId = profile.Id,
                        SignatureDataUrl = string.Empty, // Legacy
                        SignatureData = fileBytes,
                        SignatureContentType = file.ContentType,
                        IsActive = true,
                        SignedAt = dateTimeProvider.UtcNow,
                        IpAddress = ipAddress
                    };
                    context.ESignatures.Add(signature);
                    profile.ESignature = signature; // Attach locally for recalculation below
                }

                // Recalculate Profile Completion Status
                bool hasBasicInfo = !string.IsNullOrEmpty(profile.SlmcRegNumber) &&
                                    !string.IsNullOrEmpty(profile.NicNumber) &&
                                    !string.IsNullOrEmpty(profile.MobileNumber) &&
                                    !string.IsNullOrEmpty(profile.Specialty) &&
                                    profile.Qualifications != null && profile.Qualifications.Length > 0 &&
                                    profile.DateOfBirth.HasValue;

                if (hasBasicInfo)
                {
                    bool hasSlmcCert = profile.Documents.Any(d => d.Type == DocumentType.SLMC_CERT);
                    bool hasSignature = profile.ESignature != null;

                    if (hasSlmcCert && hasSignature)
                    {
                        profile.CompletionStatus = ProfileCompletionStatus.COMPLETE;
                    }
                    else
                    {
                        profile.CompletionStatus = ProfileCompletionStatus.PARTIAL;
                    }
                }
                else
                {
                    profile.CompletionStatus = ProfileCompletionStatus.MINIMAL;
                }

                await context.SaveChangesAsync(cancellationToken);

                return Results.Ok(ApiResponse<string>.CreateSuccess(relativePath));
            }
            catch (Exception ex)
            {
                return Results.Json(ApiResponse<object>.CreateFailure("Signature.Exception", $"{ex.Message}\n{ex.StackTrace}"), statusCode: 500);
            }
        })
        .DisableAntiforgery()
        .RequireAuthorization()
        .WithTags(Tags.Profile);
    }
}
