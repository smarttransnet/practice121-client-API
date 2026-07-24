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
        string normalizedNic = SriLankanNicDecoder.NormalizeNic(request.NicNumber) ?? request.NicNumber.Trim();

        // Uniqueness check for NIC
        bool nicExists = await _context.PatientAccounts
            .AnyAsync(p => p.NicNumber == normalizedNic || p.NicNumber == request.NicNumber, cancellationToken);
            
        if (nicExists)
        {
            return Result.Failure<Guid>(Error.Conflict("Patient.DuplicateNic", "An account with this NIC already exists."));
        }

        var nicDecode = SriLankanNicDecoder.DecodeNic(normalizedNic);

        DateTime? dob = null;
        if (request.DateOfBirth.HasValue)
        {
            dob = DateTime.SpecifyKind(request.DateOfBirth.Value, DateTimeKind.Utc);
        }
        else if (nicDecode.IsValid && nicDecode.DateOfBirth.HasValue)
        {
            dob = DateTime.SpecifyKind(nicDecode.DateOfBirth.Value, DateTimeKind.Utc);
        }

        string? gender = null;
        if (!string.IsNullOrWhiteSpace(request.Gender))
        {
            gender = request.Gender;
        }
        else if (nicDecode.IsValid)
        {
            gender = nicDecode.Gender;
        }

        var patient = new PatientAccount
        {
            Id = Guid.NewGuid(),
            NicNumber = normalizedNic,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = dob,
            Gender = gender,
            MobileNumber = SriLankanPhoneValidator.NormalizeToE164(request.MobileNumber) ?? request.MobileNumber,
            Verified = false,
            CompletionStatus = ProfileCompletionStatus.MINIMAL,
            CreatedByDoctorId = request.CreatedByDoctorId
        };

        _context.PatientAccounts.Add(patient);
        await _context.SaveChangesAsync(cancellationToken);

        return patient.Id;
    }
}
