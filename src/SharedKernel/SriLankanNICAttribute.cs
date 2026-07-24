using System.ComponentModel.DataAnnotations;

namespace SharedKernel;

/// <summary>
/// Validation attribute that ensures a string property is a valid Sri Lankan NIC number (Old format 9 digits + V/X or New format 12 digits).
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class SriLankanNicAttribute : ValidationAttribute
{
    public SriLankanNicAttribute()
        : base("Please enter a valid Sri Lankan NIC number.")
    {
    }

    public override bool IsValid(object? value)
    {
        // Null/empty is handled by [Required] if needed — this attribute validates format when present
        if (value is null)
        {
            return true;
        }

        if (value is not string nic)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(nic))
        {
            return true;
        }

        return SriLankanNicDecoder.IsValidNic(nic);
    }
}
