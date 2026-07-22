using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Doctors.GetMissingFields;

internal sealed class GetMissingFieldsQueryHandler : IQueryHandler<GetMissingFieldsQuery, MissingFieldsResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;

    public GetMissingFieldsQueryHandler(IApplicationDbContext context, IUserContext userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    public async Task<Result<MissingFieldsResponse>> Handle(GetMissingFieldsQuery query, CancellationToken cancellationToken)
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
            return Result.Failure<MissingFieldsResponse>(DoctorErrors.ProfileNotFound);
        }

        // 2. Identify missing fields
        var missingFields = new List<string>();

        if (string.IsNullOrWhiteSpace(profile.FullName))
        {
            missingFields.Add("fullName");
        }

        if (profile.DateOfBirth == null)
        {
            missingFields.Add("dateOfBirth");
        }

        if (string.IsNullOrWhiteSpace(profile.SlmcRegNumber))
        {
            missingFields.Add("slmcRegNumber");
        }

        if (string.IsNullOrWhiteSpace(profile.NicNumber))
        {
            missingFields.Add("nicNumber");
        }

        if (string.IsNullOrWhiteSpace(profile.MobileNumber))
        {
            missingFields.Add("mobileNumber");
        }



        if (string.IsNullOrWhiteSpace(profile.Specialty))
        {
            missingFields.Add("specialty");
        }

        // Check if an SLMC Certificate document is uploaded
        bool hasSlmcCert = profile.Documents.Any(d => d.Type == DocumentType.SLMC_CERT);
        if (!hasSlmcCert)
        {
            missingFields.Add("slmcCertificate");
        }

        // Check if an e-signature is uploaded
        if (profile.ESignature == null)
        {
            missingFields.Add("eSignature");
        }

        return new MissingFieldsResponse(missingFields, profile.CompletionStatus.ToString());
    }
}
