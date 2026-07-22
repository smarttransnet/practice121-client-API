using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Doctors.UploadDocument;

internal sealed class UploadDocumentCommandHandler : ICommandHandler<UploadDocumentCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UploadDocumentCommandHandler(
        IApplicationDbContext context, 
        IUserContext userContext,
        IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _userContext = userContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<Guid>> Handle(UploadDocumentCommand command, CancellationToken cancellationToken)
    {
        Guid accountId = _userContext.UserId;

        // 1. Fetch DoctorProfile with Documents and ESignature to check completion status
        var profile = await _context.DoctorProfiles
            .Include(p => p.Documents)
            .Include(p => p.ESignature)
            .Include(p => p.QualificationsList.Where(q => q.IsActive))
            .SingleOrDefaultAsync(p => p.AccountId == accountId, cancellationToken);

        if (profile == null)
        {
            return Result.Failure<Guid>(DoctorErrors.ProfileNotFound);
        }

        // 2. Read stream to bytes
        using var memoryStream = new MemoryStream();
        await command.FileStream.CopyToAsync(memoryStream, cancellationToken);
        byte[] fileBytes = memoryStream.ToArray();

        // 3. Mark existing documents of the same type as inactive (Archiving)
        var existingDocs = profile.Documents.Where(d => d.Type == command.Type && d.IsActive).ToList();
        foreach(var existingDoc in existingDocs)
        {
            existingDoc.IsActive = false;
            existingDoc.ArchivedAt = _dateTimeProvider.UtcNow;
        }

        // 4. Create and add Document
        var document = new Document
        {
            Id = Guid.NewGuid(),
            ProfileId = profile.Id,
            Type = command.Type,
            FileUrl = string.Empty, // Clear legacy url
            FileData = fileBytes,
            ContentType = command.ContentType,
            IsActive = true,
            UploadedAt = _dateTimeProvider.UtcNow,
            Status = DocumentStatus.PENDING
        };

        _context.Documents.Add(document);
        profile.Documents.Add(document); // Add locally for recalculation below

        // 5. Recalculate Profile Completion Status
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

        return document.Id;
    }
}
