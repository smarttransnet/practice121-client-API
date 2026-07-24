using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Doctors.UpdateProfile;

internal sealed class UpdateProfileCommandHandler : ICommandHandler<UpdateProfileCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;

    public UpdateProfileCommandHandler(IApplicationDbContext context, IUserContext userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    public async Task<Result> Handle(UpdateProfileCommand command, CancellationToken cancellationToken)
    {
        Guid accountId = _userContext.UserId;

        // 1. Fetch DoctorProfile
        var profile = await _context.DoctorProfiles
            .Include(p => p.Documents)
            .Include(p => p.ESignature)
            .Include(p => p.QualificationsList.Where(q => q.IsActive))
            .SingleOrDefaultAsync(p => p.AccountId == accountId, cancellationToken);

        if (profile == null)
        {
            return Result.Failure(DoctorErrors.ProfileNotFound);
        }

        // 2. Progressive update of profile fields
        if (!string.IsNullOrWhiteSpace(command.FullName))
        {
            profile.FullName = command.FullName;
            if (string.IsNullOrWhiteSpace(command.FirstName) && string.IsNullOrWhiteSpace(command.LastName))
            {
                var parts = command.FullName.Trim().Split(' ', 2);
                profile.FirstName = parts[0];
                profile.LastName = parts.Length > 1 ? parts[1] : string.Empty;
            }
        }

        if (command.DateOfBirth.HasValue)
        {
            profile.DateOfBirth = DateTime.SpecifyKind(command.DateOfBirth.Value, DateTimeKind.Utc);
        }

        if (command.SlmcRegNumber != null)
        {
            profile.SlmcRegNumber = string.IsNullOrWhiteSpace(command.SlmcRegNumber) ? null : command.SlmcRegNumber.Trim();
        }

        if (command.NicNumber != null)
        {
            profile.NicNumber = string.IsNullOrWhiteSpace(command.NicNumber) ? null : command.NicNumber.Trim();
        }

        if (command.MobileNumber != null)
        {
            profile.MobileNumber = string.IsNullOrWhiteSpace(command.MobileNumber)
                ? null
                : (SriLankanPhoneValidator.NormalizeToE164(command.MobileNumber) ?? command.MobileNumber.Trim());
        }

        if (command.Qualifications != null)
        {
            profile.Qualifications = command.Qualifications;
        }

        if (command.Specialty != null)
        {
            profile.Specialty = string.IsNullOrWhiteSpace(command.Specialty) ? null : command.Specialty.Trim();
        }

        if (command.SubSpecialty != null)
        {
            profile.SubSpecialty = string.IsNullOrWhiteSpace(command.SubSpecialty) ? null : command.SubSpecialty.Trim();
        }

        if (command.ProfilePictureUrl != null)
        {
            profile.ProfilePictureUrl = string.IsNullOrWhiteSpace(command.ProfilePictureUrl) ? null : command.ProfilePictureUrl.Trim();
        }

        if (command.FirstName != null)
        {
            profile.FirstName = string.IsNullOrWhiteSpace(command.FirstName) ? null : command.FirstName.Trim();
        }

        if (command.LastName != null)
        {
            profile.LastName = string.IsNullOrWhiteSpace(command.LastName) ? null : command.LastName.Trim();
        }

        if (command.Gender != null)
        {
            profile.Gender = string.IsNullOrWhiteSpace(command.Gender) ? null : command.Gender.Trim();
        }

        if (command.Bio != null)
        {
            profile.Bio = string.IsNullOrWhiteSpace(command.Bio) ? null : command.Bio.Trim();
        }

        // Update FullName for backwards compatibility
        if (command.FirstName != null || command.LastName != null)
        {
            string fName = profile.FirstName ?? string.Empty;
            string lName = profile.LastName ?? string.Empty;
            profile.FullName = $"{fName} {lName}".Trim();
        }

        // 3. Recalculate Profile Completion Status
        bool hasQualifications = profile.Qualifications != null && profile.Qualifications.Length > 0
                                 || profile.QualificationsList.Count > 0;
        bool hasBasicInfo = !string.IsNullOrEmpty(profile.SlmcRegNumber) &&
                            !string.IsNullOrEmpty(profile.NicNumber) &&
                            !string.IsNullOrEmpty(profile.MobileNumber) &&
                            !string.IsNullOrEmpty(profile.Specialty) &&
                            hasQualifications &&
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

        // 4. Save changes
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
