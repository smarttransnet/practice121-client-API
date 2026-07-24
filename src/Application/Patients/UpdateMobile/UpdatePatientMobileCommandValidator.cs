using Application.Extensions;
using FluentValidation;

namespace Application.Patients.UpdateMobile;

internal sealed class UpdatePatientMobileCommandValidator : AbstractValidator<UpdatePatientMobileCommand>
{
    public UpdatePatientMobileCommandValidator()
    {
        RuleFor(x => x.MobileNumber)
            .NotEmpty().WithMessage("Mobile Number is required")
            .MustBeValidSriLankanMobile();
    }
}
