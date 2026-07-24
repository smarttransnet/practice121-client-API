using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Patients.UpdateMobile;

public sealed record UpdatePatientMobileCommand(
    Guid PatientId,
    string MobileNumber) : ICommand;

internal sealed class UpdatePatientMobileCommandHandler(IApplicationDbContext dbContext)
    : ICommandHandler<UpdatePatientMobileCommand>
{
    public async Task<Result> Handle(UpdatePatientMobileCommand request, CancellationToken cancellationToken)
    {
        var patient = await dbContext.PatientAccounts
            .FirstOrDefaultAsync(p => p.Id == request.PatientId, cancellationToken);

        if (patient == null)
        {
            return Result.Failure(Error.NotFound("Patient.NotFound", "The patient record was not found."));
        }

        string? normalized = SriLankanPhoneValidator.NormalizeToE164(request.MobileNumber);
        if (normalized == null)
        {
            return Result.Failure(Error.Problem("Patient.InvalidMobile", "Please enter a valid Sri Lankan mobile number (e.g., 077 123 4567)."));
        }

        patient.MobileNumber = normalized;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
