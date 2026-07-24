using SharedKernel;
using Xunit;

namespace Application.UnitTests;

public class SriLankanPhoneValidatorTests
{
    [Theory]
    [InlineData("0771234567")]
    [InlineData("+94771234567")]
    [InlineData("0094771234567")]
    [InlineData("771234567")]
    [InlineData("0701234567")]
    [InlineData("0711234567")]
    [InlineData("0721234567")]
    [InlineData("0741234567")]
    [InlineData("0751234567")]
    [InlineData("0761234567")]
    [InlineData("0781234567")]
    [InlineData(" 077 123 4567 ")]
    [InlineData("077-123-4567")]
    public void IsValidLkMobile_ShouldReturnTrue_ForValidSriLankanMobileNumbers(string input)
    {
        bool result = SriLankanPhoneValidator.IsValidLkMobile(input);
        Assert.True(result);
    }

    [Theory]
    [InlineData("0112345678")] // Landline Colombo (011)
    [InlineData("0812345678")] // Landline Kandy (081)
    [InlineData("0731234567")] // Unassigned prefix 73
    [InlineData("0791234567")] // Unassigned prefix 79
    [InlineData("07712345")]   // Too short (8 digits total)
    [InlineData("077123456789")] // Too long
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("abcdefghij")]
    [InlineData("+14155552671")] // US phone number
    public void IsValidLkMobile_ShouldReturnFalse_ForInvalidNumbers(string? input)
    {
        bool result = SriLankanPhoneValidator.IsValidLkMobile(input);
        Assert.False(result);
    }

    [Theory]
    [InlineData("0771234567", "+94771234567")]
    [InlineData("+94771234567", "+94771234567")]
    [InlineData("0094771234567", "+94771234567")]
    [InlineData("771234567", "+94771234567")]
    [InlineData("0701234567", "+94701234567")]
    [InlineData("0781234567", "+94781234567")]
    [InlineData("077 123 4567", "+94771234567")]
    [InlineData("077-123-4567", "+94771234567")]
    public void NormalizeToE164_ShouldReturnE164Format_ForValidInputs(string input, string expected)
    {
        string? result = SriLankanPhoneValidator.NormalizeToE164(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("0112345678")]
    [InlineData("0731234567")]
    [InlineData("0791234567")]
    [InlineData("")]
    [InlineData(null)]
    public void NormalizeToE164_ShouldReturnNull_ForInvalidInputs(string? input)
    {
        string? result = SriLankanPhoneValidator.NormalizeToE164(input);
        Assert.Null(result);
    }

    [Fact]
    public void ValidationAttribute_ShouldValidateCorrectly()
    {
        var attr = new SriLankanMobilePhoneAttribute();

        Assert.True(attr.IsValid(null)); // Null allowed (delegates to [Required] if needed)
        Assert.True(attr.IsValid(""));
        Assert.True(attr.IsValid("0771234567"));
        Assert.False(attr.IsValid("0112345678"));
        Assert.False(attr.IsValid("0731234567"));
        Assert.False(attr.IsValid(12345678));
    }
}
