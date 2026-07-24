using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Patients.GetByMobile;

public sealed record PatientResponse(
    Guid Id,
    string NicNumber,
    string FirstName,
    string? LastName,
    DateTime? DateOfBirth,
    string? Gender,
    string MobileNumber);

public sealed record GetPatientByMobileQuery(string MobileNumber) : IQuery<PatientResponse?>;

internal sealed class GetPatientByMobileQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetPatientByMobileQuery, PatientResponse?>
{
    public async Task<Result<PatientResponse?>> Handle(GetPatientByMobileQuery request, CancellationToken cancellationToken)
    {
        string normalizedMobile = SriLankanPhoneValidator.NormalizeToE164(request.MobileNumber) ?? request.MobileNumber.Trim();
        string rawMobile = request.MobileNumber.Trim();

        var patient = await dbContext.PatientAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.MobileNumber == normalizedMobile || p.MobileNumber == rawMobile, cancellationToken);

        if (patient == null)
        {
            return Result.Success<PatientResponse?>(null);
        }

        var response = new PatientResponse(
            patient.Id,
            patient.NicNumber,
            patient.FirstName,
            patient.LastName,
            patient.DateOfBirth,
            patient.Gender,
            patient.MobileNumber);

        return Result.Success<PatientResponse?>(response);
    }
}
