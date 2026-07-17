using FluentValidation;

namespace Application.Patients.Register;

internal sealed class RegisterPatientCommandValidator : AbstractValidator<RegisterPatientCommand>
{
    public RegisterPatientCommandValidator()
    {
        RuleFor(x => x.NicNumber)
            .NotEmpty().WithMessage("NIC is required")
            .MaximumLength(20).WithMessage("NIC cannot exceed 20 characters");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First Name is required")
            .MaximumLength(100).WithMessage("First Name cannot exceed 100 characters");

        RuleFor(x => x.MobileNumber)
            .NotEmpty().WithMessage("Mobile Number is required")
            .MaximumLength(20).WithMessage("Mobile Number cannot exceed 20 characters");

        // Conditional validation based on entry point (CreatedByDoctorId)
        When(x => !x.CreatedByDoctorId.HasValue, () =>
        {
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name is required for direct registration");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of Birth is required for direct registration");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Gender is required for direct registration");
        });
    }
}
