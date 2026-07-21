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
    DateTime? VisitDate) : ICommand<Guid>;

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

        // Get max queue number/order for visitDate, filtered by practice centre and doctor
        var lastTicket = await dbContext.PatientQueueTickets
            .Where(q => q.PracticeCentreId == request.PracticeCentreId 
                        && q.DoctorId == request.DoctorId 
                        && q.VisitDate == visitDate)
            .OrderByDescending(q => q.QueueOrder)
            .FirstOrDefaultAsync(cancellationToken);

        int nextOrder = (lastTicket?.QueueOrder ?? 0) + 1;
        int nextNumber = (lastTicket?.QueueNumber ?? 0) + 1;

        var ticket = new PatientQueueTicket
        {
            Id = Guid.NewGuid(),
            QueueNumber = nextNumber,
            QueueOrder = nextOrder,
            PatientMobile = request.PatientMobile,
            DoctorId = request.DoctorId,
            PracticeCentreId = request.PracticeCentreId,
            VisitDate = visitDate,
            Status = PatientQueueStatus.Waiting,
            Priority = request.Priority,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.PatientQueueTickets.Add(ticket);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }
}
