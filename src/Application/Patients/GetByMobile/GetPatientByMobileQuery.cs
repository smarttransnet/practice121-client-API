using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Patients.GetByMobile;

public sealed record PatientResponse(
    Guid Id,
    string? NicNumber,
    string FirstName,
    string? LastName,
    DateTime? DateOfBirth,
    string? Gender,
    string MobileNumber,
    Guid? ParentId = null);

public sealed record PatientLookupResponse(
    PatientResponse PrimaryPatient,
    List<PatientResponse> Children);

public sealed record GetPatientByMobileQuery(string MobileNumber) : IQuery<PatientLookupResponse?>;

internal sealed class GetPatientByMobileQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetPatientByMobileQuery, PatientLookupResponse?>
{
    public async Task<Result<PatientLookupResponse?>> Handle(GetPatientByMobileQuery request, CancellationToken cancellationToken)
    {
        string normalizedMobile = SriLankanPhoneValidator.NormalizeToE164(request.MobileNumber) ?? request.MobileNumber.Trim();
        string rawMobile = request.MobileNumber.Trim();

        // Search for primary patient (or matching patient record)
        var primaryPatient = await dbContext.PatientAccounts
            .AsNoTracking()
            .Include(p => p.Children)
            .FirstOrDefaultAsync(p => (p.MobileNumber == normalizedMobile || p.MobileNumber == rawMobile) && p.ParentId == null, cancellationToken);

        if (primaryPatient == null)
        {
            // Fallback: check if any record matched (even if ParentId != null)
            var anyMatch = await dbContext.PatientAccounts
                .AsNoTracking()
                .Include(p => p.Children)
                .FirstOrDefaultAsync(p => p.MobileNumber == normalizedMobile || p.MobileNumber == rawMobile, cancellationToken);

            if (anyMatch == null)
            {
                return Result.Success<PatientLookupResponse?>(null);
            }

            if (anyMatch.ParentId.HasValue)
            {
                // Load parent as primary
                primaryPatient = await dbContext.PatientAccounts
                    .AsNoTracking()
                    .Include(p => p.Children)
                    .FirstOrDefaultAsync(p => p.Id == anyMatch.ParentId.Value, cancellationToken);
            }

            primaryPatient ??= anyMatch;
        }

        var primaryResponse = new PatientResponse(
            primaryPatient.Id,
            primaryPatient.NicNumber,
            primaryPatient.FirstName,
            primaryPatient.LastName,
            primaryPatient.DateOfBirth,
            primaryPatient.Gender,
            primaryPatient.MobileNumber,
            primaryPatient.ParentId);

        // Fetch children attached to this primary patient
        var childrenList = await dbContext.PatientAccounts
            .AsNoTracking()
            .Where(p => p.ParentId == primaryPatient.Id)
            .Select(c => new PatientResponse(
                c.Id,
                c.NicNumber,
                c.FirstName,
                c.LastName,
                c.DateOfBirth,
                c.Gender,
                c.MobileNumber,
                c.ParentId))
            .ToListAsync(cancellationToken);

        var lookupResponse = new PatientLookupResponse(primaryResponse, childrenList);
        return Result.Success<PatientLookupResponse?>(lookupResponse);
    }
}
