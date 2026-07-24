using System;
using Xunit;

namespace Application.UnitTests;

public class ChildPatientValidationTests
{
    public static bool IsChildAgeValid(DateTime dateOfBirth, DateTime relativeTo)
    {
        if (dateOfBirth > relativeTo)
        {
            return false;
        }

        int age = relativeTo.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > relativeTo.AddYears(-age).Date)
        {
            age--;
        }

        return age >= 0 && age < 18;
    }

    [Fact]
    public void IsChildAgeValid_ShouldReturnTrue_For5YearOld()
    {
        var relativeTo = new DateTime(2026, 7, 24, 0, 0, 0, DateTimeKind.Utc);
        var dob = relativeTo.AddYears(-5);

        Assert.True(IsChildAgeValid(dob, relativeTo));
    }

    [Fact]
    public void IsChildAgeValid_ShouldReturnTrue_For17YearOld()
    {
        var relativeTo = new DateTime(2026, 7, 24, 0, 0, 0, DateTimeKind.Utc);
        var dob = relativeTo.AddYears(-17).AddDays(1); // 17 yrs 364 days

        Assert.True(IsChildAgeValid(dob, relativeTo));
    }

    [Fact]
    public void IsChildAgeValid_ShouldReturnFalse_For18YearOld()
    {
        var relativeTo = new DateTime(2026, 7, 24, 0, 0, 0, DateTimeKind.Utc);
        var dob = relativeTo.AddYears(-18);

        Assert.False(IsChildAgeValid(dob, relativeTo));
    }

    [Fact]
    public void IsChildAgeValid_ShouldReturnFalse_ForFutureDate()
    {
        var relativeTo = new DateTime(2026, 7, 24, 0, 0, 0, DateTimeKind.Utc);
        var dob = relativeTo.AddDays(10);

        Assert.False(IsChildAgeValid(dob, relativeTo));
    }
}
