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
    PatientQueuePriority Priority) : ICommand<Guid>;

internal sealed class AddPatientQueueTicketCommandHandler(IApplicationDbContext dbContext)
    : ICommandHandler<AddPatientQueueTicketCommand, Guid>
{
    public async Task<Result<Guid>> Handle(AddPatientQueueTicketCommand request, CancellationToken cancellationToken)
    {
        // Check if doctor/practice centre exists
        var practiceCentreExists = await dbContext.PracticeCentres
            .AnyAsync(pc => pc.Id == request.PracticeCentreId, cancellationToken);
        if (!practiceCentreExists)
        {
            return Result.Failure<Guid>(new Error("PatientQueueTicket.PracticeCentreNotFound", "The specified Practice Centre does not exist.", ErrorType.NotFound));
        }

        var today = DateTime.UtcNow.Date;

        // Get max queue number/order for today, filtered by practice centre and doctor
        var lastTicket = await dbContext.PatientQueueTickets
            .Where(q => q.PracticeCentreId == request.PracticeCentreId 
                        && q.DoctorId == request.DoctorId 
                        && q.VisitDate == today)
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
            VisitDate = today,
            Status = PatientQueueStatus.Waiting,
            Priority = request.Priority,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.PatientQueueTickets.Add(ticket);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }
}
