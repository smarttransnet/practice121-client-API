using System.Globalization;
using System.Text.RegularExpressions;

namespace SharedKernel;

public sealed record NicDecodeResult(
    bool IsValid,
    DateTime? DateOfBirth,
    string? Gender,
    int? Age,
    string? NormalizedNic,
    string? ErrorMessage);

/// <summary>
/// Utility for validating, normalizing, and decoding Sri Lankan National Identity Cards (NIC).
/// Supports Old Format (10 characters: 9 digits + V/X) and New Format (12 numeric digits).
/// Extracts Date of Birth, Gender, and Age.
/// </summary>
public static partial class SriLankanNicDecoder
{
    private static readonly Regex NicPattern = CreateNicRegex();

    [GeneratedRegex(@"^(?:\d{9}[vVxX]|\d{12})$", RegexOptions.Compiled)]
    private static partial Regex CreateNicRegex();

    /// <summary>
    /// Checks whether the input is a structurally and logically valid Sri Lankan NIC number.
    /// </summary>
    public static bool IsValidNic(string? input)
    {
        return DecodeNic(input).IsValid;
    }

    /// <summary>
    /// Normalizes NIC string: trims whitespace and converts trailing 'v'/'x' to uppercase ('V'/'X').
    /// Returns null if input is null or empty.
    /// </summary>
    public static string? NormalizeNic(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        string trimmed = input.Trim();
        if (trimmed.Length == 10)
        {
            char lastChar = char.ToUpperInvariant(trimmed[9]);
            return $"{trimmed[..9]}{lastChar}";
        }

        return trimmed;
    }

    /// <summary>
    /// Decodes a Sri Lankan NIC number into DOB, Gender, Age, and Normalized NIC.
    /// </summary>
    public static NicDecodeResult DecodeNic(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return new NicDecodeResult(false, null, null, null, null, "NIC number is required.");
        }

        string? normalized = NormalizeNic(input);
        if (normalized == null || !NicPattern.IsMatch(normalized))
        {
            return new NicDecodeResult(false, null, null, null, normalized, "Please enter a valid Sri Lankan NIC number.");
        }

        int year;
        int dayOfYearRaw;

        if (normalized.Length == 10)
        {
            // Old format: 9 digits + V/X (e.g. 882441524V)
            year = 1900 + int.Parse(normalized[..2], CultureInfo.InvariantCulture);
            dayOfYearRaw = int.Parse(normalized[2..5], CultureInfo.InvariantCulture);
        }
        else
        {
            // New format: 12 digits (e.g. 199824401524)
            year = int.Parse(normalized[..4], CultureInfo.InvariantCulture);
            dayOfYearRaw = int.Parse(normalized[4..7], CultureInfo.InvariantCulture);
        }

        string gender;
        int actualDay;

        if (dayOfYearRaw > 500)
        {
            gender = "Female";
            actualDay = dayOfYearRaw - 500;
        }
        else
        {
            gender = "Male";
            actualDay = dayOfYearRaw;
        }

        bool isLeapYear = DateTime.IsLeapYear(year);
        int maxDays = isLeapYear ? 366 : 365;

        if (actualDay < 1 || actualDay > maxDays)
        {
            return new NicDecodeResult(false, null, null, null, normalized, "Invalid day component in NIC number.");
        }

        DateTime dateOfBirth;
        try
        {
            dateOfBirth = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(actualDay - 1);
        }
        catch (ArgumentOutOfRangeException)
        {
            return new NicDecodeResult(false, null, null, null, normalized, "Invalid date of birth encoded in NIC number.");
        }

        DateTime today = DateTime.UtcNow.Date;
        int age = today.Year - dateOfBirth.Year;
        if (today < dateOfBirth.AddYears(age))
        {
            age--;
        }

        return new NicDecodeResult(true, dateOfBirth, gender, age, normalized, null);
    }
}
