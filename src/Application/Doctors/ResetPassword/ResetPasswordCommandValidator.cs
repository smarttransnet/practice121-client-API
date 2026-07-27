using FluentValidation;

namespace Application.Doctors.ResetPassword;

internal sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(c => c.AccountId)
            .NotEmpty().WithMessage("Account ID is required.");

        RuleFor(c => c.Token)
            .NotEmpty().WithMessage("Reset token is required.");

        RuleFor(c => c.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.");

        RuleFor(c => c.ConfirmPassword)
            .NotEmpty().WithMessage("Please confirm your new password.")
            .Equal(c => c.NewPassword).WithMessage("Passwords do not match.");
    }
}
