using SharedKernel;

namespace Domain.Doctors;

public static class DoctorErrors
{
    public static Error NotFound(Guid id) => Error.NotFound(
        "Doctors.NotFound",
        $"The doctor account with the Id = '{id}' was not found");

    public static readonly Error NotFoundByEmail = Error.NotFound(
        "Doctors.NotFoundByEmail",
        "The doctor account with the specified email was not found");

    public static readonly Error EmailNotUnique = Error.Conflict(
        "Doctors.EmailNotUnique",
        "The provided email is already in use by another doctor account");

    public static readonly Error InvalidCredentials = Error.Problem(
        "Doctors.InvalidCredentials",
        "The provided password or email is incorrect");

    public static readonly Error OtpExpired = Error.Problem(
        "Doctors.OtpExpired",
        "The OTP verification code has expired");

    public static readonly Error OtpMaxAttemptsExceeded = Error.Problem(
        "Doctors.OtpMaxAttemptsExceeded",
        "Maximum OTP verification attempts exceeded. Session invalidated");

    public static readonly Error OtpInvalid = Error.Problem(
        "Doctors.OtpInvalid",
        "The provided OTP code is invalid");

    public static readonly Error OtpSessionNotFound = Error.NotFound(
        "Doctors.OtpSessionNotFound",
        "OTP verification session was not found or is already completed");

    public static readonly Error ProfileNotFound = Error.NotFound(
        "Doctors.ProfileNotFound",
        "The doctor profile was not found");

    public static readonly Error Unauthorized = Error.Failure(
        "Doctors.Unauthorized",
        "You are not authorized to access this resource or perform this action");

    public static readonly Error DocumentNotFound = Error.NotFound(
        "Doctors.DocumentNotFound",
        "The requested document was not found");

    public static readonly Error InvalidGoogleToken = Error.Problem(
        "Doctors.InvalidGoogleToken",
        "The provided Google ID token is invalid or expired");

    public static readonly Error BadgeNotAwarded = Error.Problem(
        "Doctors.BadgeNotAwarded",
        "The practice identity can only be generated after the doctor verification badge has been awarded");
}
