using FluentValidation;
using SharedKernel;

namespace Application.Extensions;

/// <summary>
/// FluentValidation extension methods for Sri Lankan NIC validation.
/// </summary>
public static class SriLankanNicRule
{
    private const string DefaultMessage = "Please enter a valid Sri Lankan NIC number.";

    /// <summary>
    /// Validates that the string property is a valid Sri Lankan NIC number (Old format 9 digits + V/X or New format 12 digits).
    /// </summary>
    public static IRuleBuilderOptions<T, string> MustBeValidSriLankanNic<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Must(SriLankanNicDecoder.IsValidNic)
            .WithMessage(DefaultMessage);
    }

    /// <summary>
    /// Validates that the nullable string property is a valid Sri Lankan NIC number when present.
    /// </summary>
    public static IRuleBuilderOptions<T, string?> MustBeValidSriLankanNicWhenPresent<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Must(value => string.IsNullOrWhiteSpace(value) || SriLankanNicDecoder.IsValidNic(value))
            .WithMessage(DefaultMessage);
    }
}
