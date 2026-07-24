using System.Text.RegularExpressions;

namespace SharedKernel;

/// <summary>
/// Validates and normalizes Sri Lankan mobile phone numbers to E.164 format (+947XXXXXXXX).
/// Valid mobile prefixes: 70, 71, 72, 74, 75, 76, 77, 78.
/// Excluded: landlines (011, 081, etc.) and unassigned mobile prefixes (073, 079).
/// </summary>
public static partial class SriLankanPhoneValidator
{
    // Matches: optional +94/0094/0 prefix, then 7[0-2,4-8] followed by exactly 7 digits
    private static readonly Regex LkMobileRegex = CreateLkMobileRegex();

    [GeneratedRegex(@"^(?:\+94|0094|0)?(7[01245678]\d{7})$", RegexOptions.Compiled)]
    private static partial Regex CreateLkMobileRegex();

    /// <summary>
    /// Checks whether the input is a valid Sri Lankan mobile number.
    /// Accepts formats: 0771234567, +94771234567, 0094771234567, 771234567.
    /// </summary>
    public static bool IsValidLkMobile(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        string cleaned = input.Trim().Replace(" ", "").Replace("-", "");
        return LkMobileRegex.IsMatch(cleaned);
    }

    /// <summary>
    /// Normalizes a valid Sri Lankan mobile number to E.164 format (+947XXXXXXXX).
    /// Returns null if the input is not a valid Sri Lankan mobile number.
    /// </summary>
    public static string? NormalizeToE164(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        string cleaned = input.Trim().Replace(" ", "").Replace("-", "");
        Match match = LkMobileRegex.Match(cleaned);

        if (!match.Success)
        {
            return null;
        }

        // Group 1 captures the 9-digit national number (e.g., 771234567)
        return $"+94{match.Groups[1].Value}";
    }
}
