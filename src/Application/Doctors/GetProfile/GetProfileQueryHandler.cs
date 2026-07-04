using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Doctors.GetProfile;

internal sealed class GetProfileQueryHandler : IQueryHandler<GetProfileQuery, DoctorProfileResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;

    public GetProfileQueryHandler(IApplicationDbContext context, IUserContext userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    public async Task<Result<DoctorProfileResponse>> Handle(GetProfileQuery query, CancellationToken cancellationToken)
    {
        Guid accountId = _userContext.UserId;

        // 1. Fetch DoctorAccount with all profile and verification aggregates
        DoctorAccount? account = await _context.DoctorAccounts
            .Include(a => a.Profile)
                .ThenInclude(p => p!.VerificationRecord)
            .Include(a => a.Profile)
                .ThenInclude(p => p!.Documents)
            .Include(a => a.Profile)
                .ThenInclude(p => p!.ESignature)
            .Include(a => a.PracticeIdentity)
            .SingleOrDefaultAsync(a => a.Id == accountId, cancellationToken);

        if (account == null)
        {
            return Result.Failure<DoctorProfileResponse>(DoctorErrors.NotFound(accountId));
        }

        DoctorProfile? profile = account.Profile;
        if (profile == null)
        {
            return Result.Failure<DoctorProfileResponse>(DoctorErrors.ProfileNotFound);
        }

        // 2. Map Verification Record
        VerificationRecord? vr = profile.VerificationRecord;
        bool slmcVerified = vr?.SlmcVerified ?? false;
        bool qualificationsVerified = vr?.QualificationsVerified ?? false;
        bool documentsVerified = vr?.DocumentsVerified ?? false;
        bool badgeAwarded = vr?.BadgeAwarded ?? false;

        // 3. Map Practice Identity
        string? practiceId = account.PracticeIdentity?.PracticeId;
        string? barcodeData = account.PracticeIdentity?.BarcodeData;

        // 4. Map Documents
        var documents = profile.Documents
            .Select(d => new DocumentDto(d.Id, d.Type.ToString(), d.FileUrl, d.Status.ToString(), d.UploadedAt))
            .ToList();

        // 5. Map Signature
        SignatureDto? signature = profile.ESignature != null
            ? new SignatureDto(profile.ESignature.SignatureDataUrl, profile.ESignature.SignedAt)
            : null;

        string? profilePictureUrl = profile.ProfilePictureData != null 
            ? $"/api/files/avatar/{account.Id}" 
            : profile.ProfilePictureUrl;

        // 6. Return mapped response
        var response = new DoctorProfileResponse(
            account.Id,
            account.Email,
            profile.FullName,
            profile.DateOfBirth,
            profile.SlmcRegNumber,
            profile.NicNumber,
            profile.MobileNumber,
            profile.Qualifications,
            profile.Specialty,
            profile.SubSpecialty,
            profilePictureUrl,
            profile.CompletionStatus.ToString(),
            slmcVerified,
            qualificationsVerified,
            documentsVerified,
            badgeAwarded,
            practiceId,
            barcodeData,
            documents,
            signature,
            profile.FirstName,
            profile.LastName,
            profile.Gender,
            profile.Bio
        );

        return response;
    }
}
