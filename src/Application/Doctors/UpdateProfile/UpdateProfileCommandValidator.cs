using Application.Extensions;
using FluentValidation;

namespace Application.Doctors.UpdateProfile;

internal sealed class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.FullName)
            .MaximumLength(200).WithMessage("Full Name cannot exceed 200 characters");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First Name is required")
            .MaximumLength(50).WithMessage("First Name cannot exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last Name is required")
            .MaximumLength(50).WithMessage("Last Name cannot exceed 50 characters");

        RuleFor(x => x.Bio)
            .MaximumLength(10000).WithMessage("Bio cannot exceed 10000 characters");

        RuleFor(x => x.SlmcRegNumber)
            .MaximumLength(50).WithMessage("SLMC Registration Number cannot exceed 50 characters");

        RuleFor(x => x.NicNumber)
            .MustBeValidSriLankanNicWhenPresent();

        RuleFor(x => x.MobileNumber)
            .MustBeValidSriLankanMobileWhenPresent();

        RuleFor(x => x.Specialty)
            .MaximumLength(100).WithMessage("Specialty cannot exceed 100 characters");

        RuleFor(x => x.SubSpecialty)
            .MaximumLength(100).WithMessage("SubSpecialty cannot exceed 100 characters");
    }
}
