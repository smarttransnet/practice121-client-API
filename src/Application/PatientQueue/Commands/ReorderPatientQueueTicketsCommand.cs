using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.PatientQueue.Commands;

public record ReorderPatientQueueTicketsCommand(List<Guid> TicketIds) : ICommand;

internal sealed class ReorderPatientQueueTicketsCommandHandler(IApplicationDbContext dbContext)
    : ICommandHandler<ReorderPatientQueueTicketsCommand>
{
    public async Task<Result> Handle(ReorderPatientQueueTicketsCommand request, CancellationToken cancellationToken)
    {
        if (request.TicketIds == null || request.TicketIds.Count == 0)
        {
            return Result.Success();
        }

        // Fetch all matching tickets
        var tickets = await dbContext.PatientQueueTickets
            .Where(t => request.TicketIds.Contains(t.Id))
            .ToListAsync(cancellationToken);

        // Update the order based on the position in the request list
        for (int i = 0; i < request.TicketIds.Count; i++)
        {
            var id = request.TicketIds[i];
            var ticket = tickets.FirstOrDefault(t => t.Id == id);
#pragma warning disable IDE0031
            if (ticket != null)
            {
                ticket.QueueOrder = i + 1;
            }
#pragma warning restore IDE0031
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
