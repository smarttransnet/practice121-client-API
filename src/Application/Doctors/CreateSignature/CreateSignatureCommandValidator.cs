using FluentValidation;

namespace Application.Doctors.CreateSignature;

internal sealed class CreateSignatureCommandValidator : AbstractValidator<CreateSignatureCommand>
{
    public CreateSignatureCommandValidator()
    {
        RuleFor(x => x.SignatureDataUrl)
            .NotEmpty().WithMessage("Signature image data is required")
            .Must(x => x.StartsWith("data:image/")).WithMessage("Signature must be a valid base64 image data URL");

        RuleFor(x => x.IpAddress)
            .NotEmpty().WithMessage("IP address is required");
    }
}
