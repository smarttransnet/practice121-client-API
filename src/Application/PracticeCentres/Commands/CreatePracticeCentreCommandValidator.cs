using Application.Extensions;
using FluentValidation;

namespace Application.PracticeCentres.Commands;

internal sealed class CreatePracticeCentreCommandValidator : AbstractValidator<CreatePracticeCentreCommand>
{
    public CreatePracticeCentreCommandValidator()
    {
        RuleFor(x => x.ClinicName)
            .NotEmpty().WithMessage("Clinic Name is required");

        RuleForEach(x => x.Nurses).ChildRules(nurse =>
        {
            nurse.RuleFor(n => n.Name)
                .NotEmpty().WithMessage("Nurse name is required");

            nurse.RuleFor(n => n.PhoneNumber)
                .NotEmpty().WithMessage("Nurse phone number is required")
                .MustBeValidSriLankanMobile();
        });
    }
}
