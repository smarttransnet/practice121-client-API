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

        patient.MobileNumber = request.MobileNumber.Trim();

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
