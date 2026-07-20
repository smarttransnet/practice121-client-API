using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.PatientQueue;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.PatientQueue.Commands;

public record UpdatePatientQueueTicketStatusCommand(
    Guid TicketId,
    PatientQueueStatus Status) : ICommand;

internal sealed class UpdatePatientQueueTicketStatusCommandHandler(IApplicationDbContext dbContext)
    : ICommandHandler<UpdatePatientQueueTicketStatusCommand>
{
    public async Task<Result> Handle(UpdatePatientQueueTicketStatusCommand request, CancellationToken cancellationToken)
    {
        var ticket = await dbContext.PatientQueueTickets
            .FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken);

        if (ticket == null)
        {
            return Result.Failure(new Error("PatientQueueTicket.NotFound", "The specified queue ticket does not exist.", ErrorType.NotFound));
        }

        ticket.Status = request.Status;

        if (request.Status == PatientQueueStatus.Called || request.Status == PatientQueueStatus.InConsultation)
        {
            ticket.CalledAt = DateTime.UtcNow;
        }
        else if (request.Status == PatientQueueStatus.Completed || request.Status == PatientQueueStatus.Cancelled || request.Status == PatientQueueStatus.NoShow)
        {
            ticket.CompletedAt = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
