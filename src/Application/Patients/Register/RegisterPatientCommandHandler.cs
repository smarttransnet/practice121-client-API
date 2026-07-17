using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Patients;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Domain.Doctors; // For ProfileCompletionStatus

namespace Application.Patients.Register;

internal sealed class RegisterPatientCommandHandler : ICommandHandler<RegisterPatientCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public RegisterPatientCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(RegisterPatientCommand request, CancellationToken cancellationToken)
    {
        // Uniqueness check for NIC
        bool nicExists = await _context.PatientAccounts
            .AnyAsync(p => p.NicNumber == request.NicNumber, cancellationToken);
            
        if (nicExists)
        {
            return Result.Failure<Guid>(Error.Conflict("Patient.DuplicateNic", "An account with this NIC already exists."));
        }

        var patient = new PatientAccount
        {
            Id = Guid.NewGuid(),
            NicNumber = request.NicNumber,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth.HasValue ? DateTime.SpecifyKind(request.DateOfBirth.Value, DateTimeKind.Utc) : null,
            Gender = request.Gender,
            MobileNumber = request.MobileNumber,
            Verified = false,
            CompletionStatus = ProfileCompletionStatus.MINIMAL,
            CreatedByDoctorId = request.CreatedByDoctorId
        };

        _context.PatientAccounts.Add(patient);
        await _context.SaveChangesAsync(cancellationToken);

        return patient.Id;
    }
}
