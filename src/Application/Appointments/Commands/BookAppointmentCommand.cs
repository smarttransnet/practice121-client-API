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
    Guid? PatientId = null) : ICommand<BookAppointmentResult>;

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

        // Capacity check (only if MaxPatients is set)
        if (practiceCentre.MaxPatients.HasValue)
        {
            var currentCount = await dbContext.PatientQueueTickets
                .CountAsync(t =>
                    t.PracticeCentreId == request.PracticeCentreId &&
                    t.DoctorId == request.DoctorAccountId &&
                    t.VisitDate == visitDateTime,
                    cancellationToken);

            if (currentCount >= practiceCentre.MaxPatients.Value)
            {
                return Result.Failure<BookAppointmentResult>(
                    new Error("Appointment.NoAvailability",
                        "No appointment slots are available for the selected date.",
                        ErrorType.Validation));
            }
        }

        // Get next queue number / order
        var lastTicket = await dbContext.PatientQueueTickets
            .Where(q =>
                q.PracticeCentreId == request.PracticeCentreId &&
                q.DoctorId == request.DoctorAccountId &&
                q.VisitDate == visitDateTime)
            .OrderByDescending(q => q.QueueOrder)
            .FirstOrDefaultAsync(cancellationToken);

        int nextOrder = (lastTicket?.QueueOrder ?? 0) + 1;
        int nextNumber = (lastTicket?.QueueNumber ?? 0) + 1;

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
            Status = PatientQueueStatus.Waiting,
            Priority = PatientQueuePriority.Normal,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.PatientQueueTickets.Add(ticket);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new BookAppointmentResult(ticket.Id, ticket.QueueNumber, request.VisitDate);
    }
}
