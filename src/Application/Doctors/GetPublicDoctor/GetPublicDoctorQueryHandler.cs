using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Doctors.GetPublicDoctor;

internal sealed class GetPublicDoctorQueryHandler : IQueryHandler<GetPublicDoctorQuery, PublicDoctorResponse>
{
    private readonly IApplicationDbContext _context;

    public GetPublicDoctorQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PublicDoctorResponse>> Handle(GetPublicDoctorQuery request, CancellationToken cancellationToken)
    {
        var profile = await _context.DoctorProfiles
            .AsNoTracking()
            .Where(p => p.AccountId == request.AccountId)
            .Select(p => new
            {
                p.Id,
                p.AccountId,
                p.FullName,
                p.FirstName,
                p.LastName,
                p.Specialty,
                p.SubSpecialty,
                ProfilePictureUrl = p.ProfilePictureData != null ? $"/api/files/avatar/{p.AccountId}" : p.ProfilePictureUrl,
                p.Bio
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (profile == null)
        {
            return Result.Failure<PublicDoctorResponse>(Error.NotFound("PublicDoctor.NotFound", "Doctor profile not found"));
        }

        var qualifications = await _context.Qualifications
            .AsNoTracking()
            .Where(q => q.ProfileId == profile.Id && q.IsActive)
            .Select(q => new PublicQualificationDto(q.Name))
            .ToListAsync(cancellationToken);

        return new PublicDoctorResponse(
            profile.AccountId,
            profile.FullName,
            profile.FirstName,
            profile.LastName,
            profile.Specialty,
            profile.SubSpecialty,
            profile.ProfilePictureUrl,
            profile.Bio,
            qualifications
        );
    }
}
