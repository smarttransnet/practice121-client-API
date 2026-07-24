using System;
using SharedKernel;
using Xunit;

namespace Application.UnitTests;

public class SriLankanNicDecoderTests
{
    [Fact]
    public void DecodeNic_ShouldDecodeValidOldMaleNic()
    {
        // 882441524V -> 1988, day 244 = Aug 31 1988, Male
        var result = SriLankanNicDecoder.DecodeNic("882441524V");

        Assert.True(result.IsValid);
        Assert.Equal("Male", result.Gender);
        Assert.Equal(new DateTime(1988, 8, 31, 0, 0, 0, DateTimeKind.Utc), result.DateOfBirth);
        Assert.Equal("882441524V", result.NormalizedNic);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void DecodeNic_ShouldNormalizeLowercaseTrailingV()
    {
        var result = SriLankanNicDecoder.DecodeNic("882441524v");

        Assert.True(result.IsValid);
        Assert.Equal("882441524V", result.NormalizedNic);
    }

    [Fact]
    public void DecodeNic_ShouldDecodeValidOldFemaleNic()
    {
        // 917680444X -> 1991, day 768 - 500 = 268 = Sep 25 1991, Female
        var result = SriLankanNicDecoder.DecodeNic("917680444X");

        Assert.True(result.IsValid);
        Assert.Equal("Female", result.Gender);
        Assert.Equal(new DateTime(1991, 9, 25, 0, 0, 0, DateTimeKind.Utc), result.DateOfBirth);
        Assert.Equal("917680444X", result.NormalizedNic);
    }

    [Fact]
    public void DecodeNic_ShouldDecodeValidNewMaleNic()
    {
        // 199824401524 -> 1998, day 244 = Aug 31 1998, Male
        var result = SriLankanNicDecoder.DecodeNic("199824401524");

        Assert.True(result.IsValid);
        Assert.Equal("Male", result.Gender);
        Assert.Equal(new DateTime(1998, 9, 1, 0, 0, 0, DateTimeKind.Utc), result.DateOfBirth);
        Assert.Equal("199824401524", result.NormalizedNic);
    }

    [Fact]
    public void DecodeNic_ShouldDecodeValidNewFemaleNic()
    {
        // 200176800444 -> 2001, day 768 - 500 = 268 = Sep 25 2001, Female
        var result = SriLankanNicDecoder.DecodeNic("200176800444");

        Assert.True(result.IsValid);
        Assert.Equal("Female", result.Gender);
        Assert.Equal(new DateTime(2001, 9, 25, 0, 0, 0, DateTimeKind.Utc), result.DateOfBirth);
        Assert.Equal("200176800444", result.NormalizedNic);
    }

    [Fact]
    public void DecodeNic_ShouldHandleLeapYearCorrectly()
    {
        // 960601234V -> 1996 (leap year), day 60 = Feb 29 1996
        var result = SriLankanNicDecoder.DecodeNic("960601234V");

        Assert.True(result.IsValid);
        Assert.Equal(new DateTime(1996, 2, 29, 0, 0, 0, DateTimeKind.Utc), result.DateOfBirth);
    }

    [Theory]
    [InlineData("123456789A")]   // Invalid letter (not V or X)
    [InlineData("883671524V")]   // Non-leap year 1988 day 367 -> invalid
    [InlineData("919990444X")]   // Female day 999 - 500 = 499 > 365 -> invalid
    [InlineData("12345")]        // Too short
    [InlineData("1234567890123")] // Too long
    [InlineData("")]
    [InlineData(null)]
    public void DecodeNic_ShouldReturnInvalid_ForMalformedInput(string? input)
    {
        var result = SriLankanNicDecoder.DecodeNic(input);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void IsValidNic_ShouldReturnExpectedResult()
    {
        Assert.True(SriLankanNicDecoder.IsValidNic("882441524V"));
        Assert.True(SriLankanNicDecoder.IsValidNic("199824401524"));
        Assert.False(SriLankanNicDecoder.IsValidNic("invalid_nic"));
    }

    [Fact]
    public void ValidationAttribute_ShouldValidateCorrectly()
    {
        var attr = new SriLankanNicAttribute();

        Assert.True(attr.IsValid(null));
        Assert.True(attr.IsValid(""));
        Assert.True(attr.IsValid("882441524V"));
        Assert.True(attr.IsValid("199824401524"));
        Assert.False(attr.IsValid("invalid"));
    }
}
