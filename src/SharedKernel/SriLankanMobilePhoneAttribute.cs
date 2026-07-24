using System.ComponentModel.DataAnnotations;

namespace SharedKernel;

/// <summary>
/// Validation attribute that ensures a string property is a valid Sri Lankan mobile number.
/// Can be applied to DTOs, request models, and entity properties.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class SriLankanMobilePhoneAttribute : ValidationAttribute
{
    public SriLankanMobilePhoneAttribute()
        : base("Please enter a valid Sri Lankan mobile number (e.g., 077 123 4567).")
    {
    }

    public override bool IsValid(object? value)
    {
        // Null/empty is handled by [Required] if needed — this attribute only validates format
        if (value is null)
        {
            return true;
        }

        if (value is not string phoneNumber)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return true;
        }

        return SriLankanPhoneValidator.IsValidLkMobile(phoneNumber);
    }
}
