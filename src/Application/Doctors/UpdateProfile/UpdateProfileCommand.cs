using Application.Abstractions.Messaging;

namespace Application.Doctors.UpdateProfile;

public sealed record UpdateProfileCommand(
    string FullName,
    DateTime? DateOfBirth = null,
    string? SlmcRegNumber = null,
    string? NicNumber = null,
    string? MobileNumber = null,
    string[]? Qualifications = null,
    string? Specialty = null,
    string? SubSpecialty = null,
    string? ProfilePictureUrl = null,
    string? FirstName = null, // ADDED: FirstName — new column, migration required
    string? LastName = null,  // ADDED: LastName — new column, migration required
    string? Gender = null,    // ADDED: Gender — new column, migration required
    string? Bio = null) : ICommand; // ADDED: Bio — new column, migration required
