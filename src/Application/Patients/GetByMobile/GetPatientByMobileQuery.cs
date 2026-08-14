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
    bool IsMobileOwner);

public sealed record PatientLookupResponse(
    PatientResponse? PrimaryPatient,
    List<PatientResponse> FamilyMembers);

public sealed record GetPatientByMobileQuery(
    string MobileNumber,
    string? VerificationToken = null) : IQuery<PatientLookupResponse?>;

internal sealed class GetPatientByMobileQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetPatientByMobileQuery, PatientLookupResponse?>
{
    public async Task<Result<PatientLookupResponse?>> Handle(GetPatientByMobileQuery request, CancellationToken cancellationToken)
    {
        string normalizedMobile = SriLankanPhoneValidator.NormalizeToE164(request.MobileNumber) ?? request.MobileNumber.Trim();
        string rawMobile = request.MobileNumber.Trim();

        // Security Check: Verify valid verification token
        if (string.IsNullOrWhiteSpace(request.VerificationToken))
        {
            return Result.Failure<PatientLookupResponse?>(
                Error.Problem("Otp.Required", "OTP verification is required before loading patient records."));
        }

        var otpSession = await dbContext.PatientOtpSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.VerificationToken == request.VerificationToken && s.Verified, cancellationToken);

        if (otpSession == null || otpSession.MobileNumber != normalizedMobile && otpSession.MobileNumber != rawMobile)
        {
            return Result.Failure<PatientLookupResponse?>(
                Error.Problem("Otp.Unauthorized", "Invalid or unverified OTP verification token."));
        }

        var patients = await dbContext.PatientAccounts
            .AsNoTracking()
            .Where(p => p.MobileNumber == normalizedMobile || p.MobileNumber == rawMobile)
            .ToListAsync(cancellationToken);

        if (patients.Count == 0)
        {
            return Result.Success<PatientLookupResponse?>(null);
        }

        var primaryPatientRecord = patients.FirstOrDefault(p => p.IsMobileOwner);
        var familyMemberRecords = patients.Where(p => !p.IsMobileOwner).ToList();

        PatientResponse? primaryResponse = null;
        if (primaryPatientRecord != null)
        {
            primaryResponse = new PatientResponse(
                primaryPatientRecord.Id,
                primaryPatientRecord.NicNumber,
                primaryPatientRecord.FirstName,
                primaryPatientRecord.LastName,
                primaryPatientRecord.DateOfBirth,
                primaryPatientRecord.Gender,
                primaryPatientRecord.MobileNumber,
                primaryPatientRecord.IsMobileOwner);
        }

        var familyResponses = familyMemberRecords.Select(p => new PatientResponse(
            p.Id,
            p.NicNumber,
            p.FirstName,
            p.LastName,
            p.DateOfBirth,
            p.Gender,
            p.MobileNumber,
            p.IsMobileOwner)).ToList();

        var lookupResponse = new PatientLookupResponse(primaryResponse, familyResponses);
        return Result.Success<PatientLookupResponse?>(lookupResponse);
    }
}
