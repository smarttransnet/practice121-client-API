using Application.Abstractions.Messaging;
using Domain.Doctors;

namespace Application.Doctors.UploadDocument;

public sealed record UploadDocumentCommand(DocumentType Type, Stream FileStream, string FileName, string ContentType)
    : ICommand<Guid>;
