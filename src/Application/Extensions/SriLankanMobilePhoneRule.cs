using FluentValidation;
using SharedKernel;

namespace Application.Extensions;

/// <summary>
/// FluentValidation extension methods for Sri Lankan mobile phone validation.
/// </summary>
public static class SriLankanMobilePhoneRule
{
    private const string DefaultMessage = "Please enter a valid Sri Lankan mobile number (e.g., 077 123 4567).";

    /// <summary>
    /// Validates that the string property is a valid Sri Lankan mobile number.
    /// Accepts: 0771234567, +94771234567, 0094771234567, 771234567.
    /// Rejects: landlines, invalid prefixes (073, 079), incorrect lengths.
    /// </summary>
    public static IRuleBuilderOptions<T, string> MustBeValidSriLankanMobile<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Must(SriLankanPhoneValidator.IsValidLkMobile)
            .WithMessage(DefaultMessage);
    }

    /// <summary>
    /// Validates that the nullable string property is a valid Sri Lankan mobile number (when provided).
    /// Null/empty values pass — combine with .NotEmpty() if the field is required.
    /// </summary>
    public static IRuleBuilderOptions<T, string?> MustBeValidSriLankanMobileWhenPresent<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Must(value => string.IsNullOrWhiteSpace(value) || SriLankanPhoneValidator.IsValidLkMobile(value))
            .WithMessage(DefaultMessage);
    }
}
