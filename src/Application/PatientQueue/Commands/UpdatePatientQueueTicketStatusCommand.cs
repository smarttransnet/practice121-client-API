using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.PatientQueue;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

using Application.Abstractions.Realtime;

namespace Application.PatientQueue.Commands;

public record UpdatePatientQueueTicketStatusCommand(
    Guid TicketId,
    PatientQueueStatus Status) : ICommand;

internal sealed class UpdatePatientQueueTicketStatusCommandHandler(
    IApplicationDbContext dbContext,
    IPatientQueueNotifier? notifier = null)
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

        if (request.Status == PatientQueueStatus.InConsultation && !ticket.PatientId.HasValue)
        {
            return Result.Failure(Error.Validation(
                "PatientQueueTicket.PatientIdRequired",
                "A linked patientId is required before starting consultation."));
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

        if (notifier != null)
        {
            await notifier.NotifyQueueUpdatedAsync(ticket.PracticeCentreId, ticket.DoctorId, ticket.VisitDate, cancellationToken);
        }

        return Result.Success();
    }
}
