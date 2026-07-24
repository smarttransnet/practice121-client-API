using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Patients.GetByMobile;
using Domain.Doctors;
using Domain.Patients;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Patients.AddChild;

public sealed record AddChildPatientCommand(
    Guid ParentId,
    string FullName,
    DateTime DateOfBirth,
    string Gender) : ICommand<PatientResponse>;

internal sealed class AddChildPatientCommandHandler(IApplicationDbContext dbContext)
    : ICommandHandler<AddChildPatientCommand, PatientResponse>
{
    public async Task<Result<PatientResponse>> Handle(AddChildPatientCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate Parent exists
        var parent = await dbContext.PatientAccounts
            .FirstOrDefaultAsync(p => p.Id == request.ParentId, cancellationToken);

        if (parent == null)
        {
            return Result.Failure<PatientResponse>(
                Error.NotFound("Patient.ParentNotFound", "The primary parent account was not found."));
        }

        // 2. Validate Full Name
        string fullName = request.FullName?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return Result.Failure<PatientResponse>(
                Error.Validation("Patient.ChildNameRequired", "Child full name is required."));
        }

        // Split Full Name into FirstName and LastName
        string[] nameParts = fullName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        string firstName = nameParts.Length > 0 ? nameParts[0] : fullName;
        string? lastName = nameParts.Length > 1 ? nameParts[1] : null;

        // 3. Validate Date of Birth (must be in past and age < 18 years)
        var utcDob = DateTime.SpecifyKind(request.DateOfBirth, DateTimeKind.Utc);
        var now = DateTime.UtcNow;

        if (utcDob > now)
        {
            return Result.Failure<PatientResponse>(
                Error.Validation("Patient.InvalidDateOfBirth", "Date of birth must be in the past."));
        }

        // Calculate age
        int age = now.Year - utcDob.Year;
        if (utcDob.Date > now.AddYears(-age).Date)
        {
            age--;
        }

        if (age >= 18)
        {
            return Result.Failure<PatientResponse>(
                Error.Validation("Patient.ChildAgeExceeded", "Child patient must be under 18 years old."));
        }

        // 4. Validate Gender
        string gender = request.Gender?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(gender))
        {
            return Result.Failure<PatientResponse>(
                Error.Validation("Patient.GenderRequired", "Gender is required."));
        }

        // 5. Create Child Patient record
        var child = new PatientAccount
        {
            Id = Guid.NewGuid(),
            ParentId = parent.Id,
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = utcDob,
            Gender = gender,
            MobileNumber = parent.MobileNumber, // Inherit primary parent contact mobile
            NicNumber = null,
            Verified = false,
            CompletionStatus = ProfileCompletionStatus.MINIMAL,
            CreatedByDoctorId = parent.CreatedByDoctorId
        };

        dbContext.PatientAccounts.Add(child);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new PatientResponse(
            child.Id,
            child.NicNumber,
            child.FirstName,
            child.LastName,
            child.DateOfBirth,
            child.Gender,
            child.MobileNumber,
            child.ParentId);

        return Result.Success(response);
    }
}
