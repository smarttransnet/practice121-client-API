using Application.Abstractions.Messaging;
using Domain.Doctors;

namespace Application.Patients.UploadDocument;

public sealed record UploadPatientDocumentCommand(
    Guid PatientId,
    DocumentType Type,
    Stream FileStream,
    string ContentType
) : ICommand<Guid>;
