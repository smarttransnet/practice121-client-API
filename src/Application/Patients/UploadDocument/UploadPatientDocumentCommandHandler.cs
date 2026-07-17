using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Doctors;
using Domain.Patients;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Patients.UploadDocument;

internal sealed class UploadPatientDocumentCommandHandler : ICommandHandler<UploadPatientDocumentCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UploadPatientDocumentCommandHandler(
        IApplicationDbContext context, 
        IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<Guid>> Handle(UploadPatientDocumentCommand request, CancellationToken cancellationToken)
    {
        var patient = await _context.PatientAccounts
            .Include(p => p.Documents)
            .SingleOrDefaultAsync(p => p.Id == request.PatientId, cancellationToken);

        if (patient == null)
        {
            return Result.Failure<Guid>(Error.NotFound("Patient.NotFound", "Patient account not found."));
        }

        using var memoryStream = new MemoryStream();
        await request.FileStream.CopyToAsync(memoryStream, cancellationToken);
        byte[] fileBytes = memoryStream.ToArray();

        // Mark existing documents of the same type as inactive
        var existingDocs = patient.Documents.Where(d => d.Type == request.Type && d.IsActive).ToList();
        foreach (var existingDoc in existingDocs)
        {
            existingDoc.IsActive = false;
            existingDoc.ArchivedAt = _dateTimeProvider.UtcNow;
        }

        var document = new PatientDocument
        {
            Id = Guid.NewGuid(),
            PatientAccountId = patient.Id,
            Type = request.Type,
            FileData = fileBytes,
            ContentType = request.ContentType,
            IsActive = true,
            UploadedAt = _dateTimeProvider.UtcNow,
            Status = DocumentStatus.PENDING
        };

        _context.PatientDocuments.Add(document);
        patient.Documents.Add(document);

        // Recalculate Profile Completion Status
        // For patients, we only care if they have NIC uploaded or not based on our current requirements
        bool hasNic = patient.Documents.Any(d => d.Type == DocumentType.NIC && d.IsActive);
        
        if (hasNic)
        {
            patient.CompletionStatus = ProfileCompletionStatus.COMPLETE;
        }
        else
        {
            patient.CompletionStatus = ProfileCompletionStatus.MINIMAL;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return document.Id;
    }
}
