using Application.Extensions;
using FluentValidation;

namespace Application.PatientQueue.Commands;

internal sealed class AddPatientQueueTicketCommandValidator : AbstractValidator<AddPatientQueueTicketCommand>
{
    public AddPatientQueueTicketCommandValidator()
    {
        RuleFor(x => x.PatientMobile)
            .NotEmpty().WithMessage("Patient mobile number is required")
            .MustBeValidSriLankanMobile();

        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("Doctor ID is required");

        RuleFor(x => x.PracticeCentreId)
            .NotEmpty().WithMessage("Practice centre ID is required");
    }
}
