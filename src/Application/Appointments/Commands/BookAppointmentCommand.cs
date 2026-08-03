using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.PatientQueue;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Appointments.Commands;

public record BookAppointmentCommand(
    string PatientMobile,
    Guid DoctorAccountId,
    Guid PracticeCentreId,
    DateOnly VisitDate,
    Guid? PatientId = null,
    Guid? SessionId = null) : ICommand<BookAppointmentResult>;

public record BookAppointmentResult(
    Guid TicketId,
    int QueueNumber,
    DateOnly VisitDate);

internal sealed class BookAppointmentCommandHandler(IApplicationDbContext dbContext)
    : ICommandHandler<BookAppointmentCommand, BookAppointmentResult>
{
    public async Task<Result<BookAppointmentResult>> Handle(
        BookAppointmentCommand request,
        CancellationToken cancellationToken)
    {
        // Load practice centre with session groups
        var practiceCentre = await dbContext.PracticeCentres
            .Include(pc => pc.SessionGroups)
            .FirstOrDefaultAsync(
                pc => pc.Id == request.PracticeCentreId && pc.DoctorId == request.DoctorAccountId,
                cancellationToken);

        if (practiceCentre is null)
        {
            return Result.Failure<BookAppointmentResult>(
                new Error("Appointment.PracticeCentreNotFound",
                    "The specified practice centre was not found.",
                    ErrorType.NotFound));
        }

        // Validate session availability for chosen day of week
        var dayAbbr = request.VisitDate.DayOfWeek.ToString()[..3].ToUpperInvariant();
        var hasSession = practiceCentre.SessionGroups
            .Any(sg => sg.DaysOfWeek.Any(d => d.Equals(dayAbbr, StringComparison.OrdinalIgnoreCase)));

        if (!hasSession)
        {
            return Result.Failure<BookAppointmentResult>(
                new Error("Appointment.NoSessionOnSelectedDate",
                    "No session is scheduled for the selected date.",
                    ErrorType.Validation));
        }

        var visitDateTime = request.VisitDate.ToDateTime(TimeOnly.MinValue);

        // Auto-assign session ID if not explicitly provided
        var effectiveSessionId = request.SessionId;
        if (!effectiveSessionId.HasValue || effectiveSessionId.Value == Guid.Empty)
        {
            var matchingSessionGroup = practiceCentre.SessionGroups
                .FirstOrDefault(sg => sg.DaysOfWeek.Any(d => d.Equals(dayAbbr, StringComparison.OrdinalIgnoreCase)));
            if (matchingSessionGroup != null)
            {
                effectiveSessionId = matchingSessionGroup.TimeBlocks.FirstOrDefault()?.Id ?? matchingSessionGroup.Id;
            }
        }

        // Capacity check per session (only if MaxPatients is set)
        if (practiceCentre.MaxPatients.HasValue)
        {
            var countQuery = dbContext.PatientQueueTickets
                .Where(t =>
                    t.PracticeCentreId == request.PracticeCentreId &&
                    t.DoctorId == request.DoctorAccountId &&
                    t.VisitDate == visitDateTime &&
                    t.Status != PatientQueueStatus.Cancelled);

            if (effectiveSessionId.HasValue && effectiveSessionId.Value != Guid.Empty)
            {
                countQuery = countQuery.Where(t => t.SessionId == effectiveSessionId.Value);
            }

            var currentCount = await countQuery.CountAsync(cancellationToken);

            if (currentCount >= practiceCentre.MaxPatients.Value)
            {
                return Result.Failure<BookAppointmentResult>(
                    new Error("Appointment.NoAvailability",
                        "No appointment slots are available for the selected session.",
                        ErrorType.Validation));
            }
        }

        // Get next queue order globally for visitDate
        var lastOrderTicket = await dbContext.PatientQueueTickets
            .Where(q =>
                q.PracticeCentreId == request.PracticeCentreId &&
                q.DoctorId == request.DoctorAccountId &&
                q.VisitDate == visitDateTime)
            .OrderByDescending(q => q.QueueOrder)
            .FirstOrDefaultAsync(cancellationToken);

        int nextOrder = (lastOrderTicket?.QueueOrder ?? 0) + 1;

        // Queue number sequence per session
        int nextNumber = 1;
        if (effectiveSessionId.HasValue && effectiveSessionId.Value != Guid.Empty)
        {
            var lastSessionTicket = await dbContext.PatientQueueTickets
                .Where(q => q.PracticeCentreId == request.PracticeCentreId
                            && q.DoctorId == request.DoctorAccountId
                            && q.VisitDate == visitDateTime
                            && q.SessionId == effectiveSessionId.Value)
                .OrderByDescending(q => q.QueueNumber)
                .FirstOrDefaultAsync(cancellationToken);

            nextNumber = (lastSessionTicket?.QueueNumber ?? 0) + 1;
        }
        else
        {
            var lastTicket = await dbContext.PatientQueueTickets
                .Where(q => q.PracticeCentreId == request.PracticeCentreId
                            && q.DoctorId == request.DoctorAccountId
                            && q.VisitDate == visitDateTime)
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
            DoctorId = request.DoctorAccountId,
            PracticeCentreId = request.PracticeCentreId,
            VisitDate = visitDateTime,
            SessionId = effectiveSessionId,
            Status = PatientQueueStatus.Waiting,
            Priority = PatientQueuePriority.Normal,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.PatientQueueTickets.Add(ticket);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new BookAppointmentResult(ticket.Id, ticket.QueueNumber, request.VisitDate);
    }
}
