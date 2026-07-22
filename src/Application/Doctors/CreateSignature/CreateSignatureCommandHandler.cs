using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Doctors.CreateSignature;

internal sealed class CreateSignatureCommandHandler : ICommandHandler<CreateSignatureCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateSignatureCommandHandler(
        IApplicationDbContext context, 
        IUserContext userContext,
        IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _userContext = userContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result> Handle(CreateSignatureCommand command, CancellationToken cancellationToken)
    {
        Guid accountId = _userContext.UserId;

        // 1. Fetch DoctorProfile with Aggregates
        var profile = await _context.DoctorProfiles
            .Include(p => p.Documents)
            .Include(p => p.ESignature)
            .Include(p => p.QualificationsList.Where(q => q.IsActive))
            .SingleOrDefaultAsync(p => p.AccountId == accountId, cancellationToken);

        if (profile == null)
        {
            return Result.Failure(DoctorErrors.ProfileNotFound);
        }

        // 2. Create or Update ESignature
        if (profile.ESignature != null)
        {
            profile.ESignature.SignatureDataUrl = command.SignatureDataUrl;
            profile.ESignature.SignedAt = _dateTimeProvider.UtcNow;
            profile.ESignature.IpAddress = command.IpAddress;
        }
        else
        {
            var signature = new ESignature
            {
                Id = Guid.NewGuid(),
                ProfileId = profile.Id,
                SignatureDataUrl = command.SignatureDataUrl,
                SignedAt = _dateTimeProvider.UtcNow,
                IpAddress = command.IpAddress
            };
            _context.ESignatures.Add(signature);
            profile.ESignature = signature; // Attach locally for recalculation below
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

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
