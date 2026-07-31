using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Realtime;
using Microsoft.AspNetCore.SignalR;
using Web.Api.Hubs;

namespace Web.Api.Infrastructure;

public class PatientQueueNotifier(IHubContext<PatientQueueHub> hubContext) : IPatientQueueNotifier
{
    public async Task NotifyQueueUpdatedAsync(Guid practiceCentreId, Guid? doctorId = null, DateTime? visitDate = null, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            practiceCentreId,
            doctorId,
            visitDate = visitDate?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            timestamp = DateTime.UtcNow
        };

        // Broadcast to group for practiceCentreId AND to all connected clients
        await hubContext.Clients.Group(practiceCentreId.ToString().ToLowerInvariant())
            .SendAsync("QueueUpdated", payload, cancellationToken);

        await hubContext.Clients.All
            .SendAsync("QueueUpdated", payload, cancellationToken);
    }
}
