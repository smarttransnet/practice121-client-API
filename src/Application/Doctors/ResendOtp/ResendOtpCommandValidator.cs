using FluentValidation;

namespace Application.Doctors.ResendOtp;

internal sealed class ResendOtpCommandValidator : AbstractValidator<ResendOtpCommand>
{
    public ResendOtpCommandValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("Account ID is required");

        RuleFor(x => x.Channel)
            .IsInEnum().WithMessage("A valid OTP channel must be specified");
    }
}
