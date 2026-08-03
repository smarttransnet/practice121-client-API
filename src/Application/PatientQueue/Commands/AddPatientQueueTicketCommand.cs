using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.PatientQueue;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.PatientQueue.Commands;

public record AddPatientQueueTicketCommand(
    string PatientMobile,
    Guid DoctorId,
    Guid PracticeCentreId,
    PatientQueuePriority Priority,
    DateTime? VisitDate,
    Guid? PatientId = null,
    Guid? SessionId = null) : ICommand<Guid>;

internal sealed class AddPatientQueueTicketCommandHandler(IApplicationDbContext dbContext)
    : ICommandHandler<AddPatientQueueTicketCommand, Guid>
{
    public async Task<Result<Guid>> Handle(AddPatientQueueTicketCommand request, CancellationToken cancellationToken)
    {
        // Check if doctor/practice centre exists
        var practiceCentre = await dbContext.PracticeCentres
            .Include(pc => pc.SessionGroups)
            .FirstOrDefaultAsync(pc => pc.Id == request.PracticeCentreId, cancellationToken);
        if (practiceCentre == null)
        {
            return Result.Failure<Guid>(new Error("PatientQueueTicket.PracticeCentreNotFound", "The specified Practice Centre does not exist.", ErrorType.NotFound));
        }

        var visitDate = request.VisitDate?.Date ?? DateTime.UtcNow.Date;

        // Validate doctor availability for selected date
        var dayOfWeekString = visitDate.DayOfWeek.ToString().Substring(0, 3).ToUpperInvariant();
        var hasSession = practiceCentre.SessionGroups.Any(sg => 
            sg.DaysOfWeek.Any(d => d.Equals(dayOfWeekString, StringComparison.OrdinalIgnoreCase)));
        if (!hasSession)
        {
            return Result.Failure<Guid>(new Error("PatientQueueTicket.NoSessionOnSelectedDate", "No session group is scheduled for the selected date's day of week.", ErrorType.Validation));
        }

        // Auto-assign session ID if not explicitly provided
        var effectiveSessionId = request.SessionId;
        if (!effectiveSessionId.HasValue || effectiveSessionId.Value == Guid.Empty)
        {
            var matchingSessionGroup = practiceCentre.SessionGroups
                .FirstOrDefault(sg => sg.DaysOfWeek.Any(d => d.Equals(dayOfWeekString, StringComparison.OrdinalIgnoreCase)));
            if (matchingSessionGroup != null)
            {
                effectiveSessionId = matchingSessionGroup.TimeBlocks.FirstOrDefault()?.Id ?? matchingSessionGroup.Id;
            }
        }

        // Prevent duplicate patient record in the same session
        if (request.PatientId.HasValue && request.PatientId.Value != Guid.Empty)
        {
            var isDuplicatePatient = await dbContext.PatientQueueTickets.AnyAsync(q =>
                q.PracticeCentreId == request.PracticeCentreId
                && q.DoctorId == request.DoctorId
                && q.VisitDate == visitDate
                && q.SessionId == effectiveSessionId
                && q.PatientId == request.PatientId.Value
                && q.Status != PatientQueueStatus.Cancelled
                && q.Status != PatientQueueStatus.Completed
                && q.Status != PatientQueueStatus.NoShow,
                cancellationToken);

            if (isDuplicatePatient)
            {
                return Result.Failure<Guid>(new Error("PatientQueueTicket.DuplicatePatientRecord", "This patient record is already in the queue for the selected session.", ErrorType.Validation));
            }
        }
        else
        {
            var normalizedMobile = SriLankanPhoneValidator.NormalizeToE164(request.PatientMobile) ?? request.PatientMobile;
            var isDuplicateMobile = await dbContext.PatientQueueTickets.AnyAsync(q =>
                q.PracticeCentreId == request.PracticeCentreId
                && q.DoctorId == request.DoctorId
                && q.VisitDate == visitDate
                && q.SessionId == effectiveSessionId
                && q.PatientMobile == normalizedMobile
                && q.PatientId == null
                && q.Status != PatientQueueStatus.Cancelled
                && q.Status != PatientQueueStatus.Completed
                && q.Status != PatientQueueStatus.NoShow,
                cancellationToken);

            if (isDuplicateMobile)
            {
                return Result.Failure<Guid>(new Error("PatientQueueTicket.DuplicatePatientRecord", "A patient with this mobile number is already in the queue for the selected session.", ErrorType.Validation));
            }
        }

        // Get last ticket order globally for visitDate
        var lastOrderTicket = await dbContext.PatientQueueTickets
            .Where(q => q.PracticeCentreId == request.PracticeCentreId 
                        && q.DoctorId == request.DoctorId 
                        && q.VisitDate == visitDate)
            .OrderByDescending(q => q.QueueOrder)
            .FirstOrDefaultAsync(cancellationToken);

        int nextOrder = (lastOrderTicket?.QueueOrder ?? 0) + 1;

        // Queue number sequence is separate for each session on that visitDate
        int nextNumber = 1;
        if (effectiveSessionId.HasValue && effectiveSessionId.Value != Guid.Empty)
        {
            var lastSessionTicket = await dbContext.PatientQueueTickets
                .Where(q => q.PracticeCentreId == request.PracticeCentreId 
                            && q.DoctorId == request.DoctorId 
                            && q.VisitDate == visitDate
                            && q.SessionId == effectiveSessionId.Value)
                .OrderByDescending(q => q.QueueNumber)
                .FirstOrDefaultAsync(cancellationToken);

            nextNumber = (lastSessionTicket?.QueueNumber ?? 0) + 1;
        }
        else
        {
            var lastTicket = await dbContext.PatientQueueTickets
                .Where(q => q.PracticeCentreId == request.PracticeCentreId 
                            && q.DoctorId == request.DoctorId 
                            && q.VisitDate == visitDate)
                .OrderByDescending(q => q.QueueNumber)
                .FirstOrDefaultAsync(cancellationToken);

            nextNumber = (lastTicket?.QueueNumber ?? 0) + 1;
        }

        var ticket = new PatientQueueTicket
        {
            Id = Guid.NewGuid(),
            QueueNumber = nextNumber,
            QueueOrder = nextOrder,
            PatientMobile = SriLankanPhoneValidator.NormalizeToE164(request.PatientMobile) ?? request.PatientMobile,
            PatientId = request.PatientId,
            DoctorId = request.DoctorId,
            PracticeCentreId = request.PracticeCentreId,
            VisitDate = visitDate,
            SessionId = effectiveSessionId,
            Status = PatientQueueStatus.Waiting,
            Priority = request.Priority,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.PatientQueueTickets.Add(ticket);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }
}
