using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Domain.Doctors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Profile;

internal sealed class Qualifications : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/profile/qualifications", async (
            IApplicationDbContext context,
            IUserContext userContext,
            CancellationToken cancellationToken) =>
        {
            Guid accountId = userContext.UserId;
            var profile = await context.DoctorProfiles
                .Include(p => p.QualificationsList.Where(q => q.IsActive))
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.AccountId == accountId, cancellationToken);

            if (profile == null)
            {
                return Results.NotFound(ApiResponse<object>.CreateFailure("Qualifications.ProfileNotFound", "Doctor profile not found"));
            }

            var result = profile.QualificationsList.Select(q => new 
            {
                q.Id,
                q.Name,
                HasCertificate = q.CertificateData != null && q.CertificateData.Length > 0,
                CertificateUrl = q.CertificateData != null && q.CertificateData.Length > 0 ? $"/api/files/qualification/{q.Id}" : null
            });

            return Results.Ok(ApiResponse<object>.CreateSuccess(result));
        })
        .RequireAuthorization()
        .WithTags(Tags.Profile);

        app.MapPost("api/profile/qualifications", async (
            [FromForm] string name,
            IFormFile? file,
            IApplicationDbContext context,
            IUserContext userContext,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Results.BadRequest(ApiResponse<object>.CreateFailure("Qualifications.InvalidName", "Qualification name is required"));
            }

            Guid accountId = userContext.UserId;
            var profile = await context.DoctorProfiles
                .SingleOrDefaultAsync(p => p.AccountId == accountId, cancellationToken);

            if (profile == null)
            {
                return Results.NotFound(ApiResponse<object>.CreateFailure("Qualifications.ProfileNotFound", "Doctor profile not found"));
            }

            var qualification = new Qualification
            {
                Id = Guid.NewGuid(),
                ProfileId = profile.Id,
                Name = name,
                IsActive = true
            };

            if (file != null && file.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream, cancellationToken);
                qualification.CertificateData = memoryStream.ToArray();
                qualification.CertificateContentType = file.ContentType;
            }

            context.Qualifications.Add(qualification);
            await context.SaveChangesAsync(cancellationToken);

            var result = new 
            {
                qualification.Id,
                qualification.Name,
                HasCertificate = qualification.CertificateData != null && qualification.CertificateData.Length > 0,
                CertificateUrl = qualification.CertificateData != null && qualification.CertificateData.Length > 0 ? $"/api/files/qualification/{qualification.Id}" : null
            };

            return Results.Ok(ApiResponse<object>.CreateSuccess(result));
        })
        .DisableAntiforgery()
        .RequireAuthorization()
        .WithTags(Tags.Profile);

        app.MapDelete("api/profile/qualifications/{id:guid}", async (
            Guid id,
            IApplicationDbContext context,
            IUserContext userContext,
            IDateTimeProvider dateTimeProvider,
            CancellationToken cancellationToken) =>
        {
            Guid accountId = userContext.UserId;
            var profile = await context.DoctorProfiles
                .SingleOrDefaultAsync(p => p.AccountId == accountId, cancellationToken);

            if (profile == null)
            {
                return Results.NotFound();
            }

            var qualification = await context.Qualifications
                .SingleOrDefaultAsync(q => q.Id == id && q.ProfileId == profile.Id, cancellationToken);

            if (qualification != null)
            {
                qualification.IsActive = false;
                qualification.ArchivedAt = dateTimeProvider.UtcNow;
                await context.SaveChangesAsync(cancellationToken);
            }

            return Results.Ok(ApiResponse<object>.CreateSuccess(new object()));
        })
        .RequireAuthorization()
        .WithTags(Tags.Profile);
    }
}
