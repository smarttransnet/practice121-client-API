using Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;

namespace Application.Doctors.GetPublicDoctor;

public sealed record PublicQualificationDto(string Name);

public sealed record PublicDoctorResponse(
    Guid AccountId,
    string FullName,
    string? FirstName,
    string? LastName,
    string? Specialty,
    string? SubSpecialty,
    string? ProfilePictureUrl,
    string? Bio,
    List<PublicQualificationDto> Qualifications);

public sealed record GetPublicDoctorQuery(Guid AccountId) : IQuery<PublicDoctorResponse>;
