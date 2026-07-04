using Application.Abstractions.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.VerifyProcess;

internal sealed class GetDoctors : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/verify-process/doctors", async (
            IApplicationDbContext context,
            CancellationToken cancellationToken) =>
        {
            var doctors = await context.DoctorProfiles
                .AsNoTracking()
                .Select(p => new
                {
                    p.Id,
                    p.FullName,
                    p.SlmcRegNumber,
                    p.Specialty,
                    p.CompletionStatus,
                    AccountEmail = p.Account != null ? p.Account.Email : null
                })
                .ToListAsync(cancellationToken);

            return Results.Ok(ApiResponse<object>.CreateSuccess(doctors));
        })
        .RequireAuthorization()
        .WithTags("VerifyProcess");
    }
}
