using Application.Extensions;
using FluentValidation;

namespace Application.Appointments.Commands;

internal sealed class BookAppointmentCommandValidator : AbstractValidator<BookAppointmentCommand>
{
    public BookAppointmentCommandValidator()
    {
        RuleFor(x => x.PatientMobile)
            .NotEmpty().WithMessage("Patient mobile number is required")
            .MustBeValidSriLankanMobile();

        RuleFor(x => x.DoctorAccountId)
            .NotEmpty().WithMessage("Doctor account ID is required");

        RuleFor(x => x.PracticeCentreId)
            .NotEmpty().WithMessage("Practice centre ID is required");
    }
}
