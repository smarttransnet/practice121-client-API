using Application.Abstractions.Messaging;
using Application.Doctors.GetProfile;
using Application.Doctors.UpdateProfile;
using SharedKernel;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Profile;

internal sealed class Me : IEndpoint
{
    public sealed record UpdateRequest(
        string FullName,
        DateTime? DateOfBirth,
        string? SlmcRegNumber,
        string? NicNumber,
        string? MobileNumber,
        string[]? Qualifications,
        string? Specialty,
        string? SubSpecialty,
        string? ProfilePictureUrl,
        string? FirstName, // ADDED: FirstName — new column, migration required
        string? LastName,  // ADDED: LastName — new column, migration required
        string? Gender,    // ADDED: Gender — new column, migration required
        string? Bio);      // ADDED: Bio — new column, migration required

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // GET /api/profile/me
        app.MapGet("api/profile/me", async (
            IQueryHandler<GetProfileQuery, DoctorProfileResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetProfileQuery();

            Result<DoctorProfileResponse> result = await handler.Handle(query, cancellationToken);

            return result.ToApiResponse();
        })
        .WithName("GetMyProfile")
        .RequireAuthorization()
        .WithTags(Tags.Profile);

        // PATCH /api/profile/me
        app.MapPatch("api/profile/me", async (
            UpdateRequest request,
            ICommandHandler<UpdateProfileCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateProfileCommand(
                request.FullName,
                request.DateOfBirth,
                request.SlmcRegNumber,
                request.NicNumber,
                request.MobileNumber,
                request.Qualifications,
                request.Specialty,
                request.SubSpecialty,
                request.ProfilePictureUrl,
                request.FirstName,
                request.LastName,
                request.Gender,
                request.Bio);

            Result result = await handler.Handle(command, cancellationToken);

            return result.ToApiResponse();
        })
        .WithName("UpdateMyProfile")
        .RequireAuthorization()
        .WithTags(Tags.Profile);
    }
}
