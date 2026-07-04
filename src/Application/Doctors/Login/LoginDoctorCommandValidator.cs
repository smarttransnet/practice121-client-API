using FluentValidation;

namespace Application.Doctors.Login;

internal sealed class LoginDoctorCommandValidator : AbstractValidator<LoginDoctorCommand>
{
    public LoginDoctorCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("A valid email address is required");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}
