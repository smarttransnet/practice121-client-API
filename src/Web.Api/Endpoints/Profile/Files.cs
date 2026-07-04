using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Profile;

internal sealed class Files : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/files/avatar/{accountId:guid}", async (
            Guid accountId,
            IApplicationDbContext context,
            CancellationToken cancellationToken) =>
        {
            var profile = await context.DoctorProfiles
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.AccountId == accountId, cancellationToken);

            if (profile == null || profile.ProfilePictureData == null || profile.ProfilePictureData.Length == 0)
            {
                // Fallback to legacy URL or 404
                if (!string.IsNullOrEmpty(profile?.ProfilePictureUrl))
                {
                    return Results.Redirect(profile.ProfilePictureUrl);
                }
                return Results.NotFound();
            }

            return Results.File(profile.ProfilePictureData, profile.ProfilePictureContentType ?? "image/jpeg");
        })
        .DisableAntiforgery()
        .AllowAnonymous()
        .WithTags(Tags.Profile);

        app.MapGet("api/files/signature/{accountId:guid}", async (
            Guid accountId,
            IApplicationDbContext context,
            CancellationToken cancellationToken) =>
        {
            var profile = await context.DoctorProfiles
                .Include(p => p.ESignature)
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.AccountId == accountId, cancellationToken);

            if (profile?.ESignature == null || profile.ESignature.SignatureData == null || profile.ESignature.SignatureData.Length == 0)
            {
                if (!string.IsNullOrEmpty(profile?.ESignature?.SignatureDataUrl))
                {
                    return Results.Redirect(profile.ESignature.SignatureDataUrl);
                }
                return Results.NotFound();
            }

            return Results.File(profile.ESignature.SignatureData, profile.ESignature.SignatureContentType ?? "image/png");
        })
        .DisableAntiforgery()
        .WithTags(Tags.Profile);

        app.MapGet("api/files/document/{documentId:guid}", async (
            Guid documentId,
            IApplicationDbContext context,
            CancellationToken cancellationToken) =>
        {
            var document = await context.Documents
                .AsNoTracking()
                .SingleOrDefaultAsync(d => d.Id == documentId, cancellationToken);

            if (document == null || document.FileData == null || document.FileData.Length == 0)
            {
                if (!string.IsNullOrEmpty(document?.FileUrl))
                {
                    return Results.Redirect(document.FileUrl);
                }
                return Results.NotFound();
            }

            return Results.File(document.FileData, document.ContentType ?? "application/octet-stream");
        })
        .DisableAntiforgery()
        .WithTags(Tags.Profile);

        app.MapGet("api/files/qualification/{qualificationId:guid}", async (
            Guid qualificationId,
            IApplicationDbContext context,
            CancellationToken cancellationToken) =>
        {
            var qualification = await context.Qualifications
                .AsNoTracking()
                .SingleOrDefaultAsync(q => q.Id == qualificationId, cancellationToken);

            if (qualification == null || qualification.CertificateData == null || qualification.CertificateData.Length == 0)
            {
                return Results.NotFound();
            }

            return Results.File(qualification.CertificateData, qualification.CertificateContentType ?? "application/octet-stream");
        })
        .DisableAntiforgery()
        .WithTags(Tags.Profile);
    }
}
