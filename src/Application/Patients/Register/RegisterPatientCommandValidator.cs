using Application.Extensions;
using FluentValidation;

namespace Application.Patients.Register;

internal sealed class RegisterPatientCommandValidator : AbstractValidator<RegisterPatientCommand>
{
    public RegisterPatientCommandValidator()
    {
        RuleFor(x => x.NicNumber)
            .NotEmpty().WithMessage("NIC is required")
            .MustBeValidSriLankanNic();

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First Name is required")
            .MaximumLength(100).WithMessage("First Name cannot exceed 100 characters");

        RuleFor(x => x.MobileNumber)
            .NotEmpty().WithMessage("Mobile Number is required")
            .MustBeValidSriLankanMobile();

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
