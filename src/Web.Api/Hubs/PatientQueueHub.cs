using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Web.Api.Hubs;

public class PatientQueueHub : Hub
{
    public async Task JoinQueueGroup(string practiceCentreId)
    {
        if (!string.IsNullOrWhiteSpace(practiceCentreId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, practiceCentreId.ToLowerInvariant());
        }
    }

    public async Task LeaveQueueGroup(string practiceCentreId)
    {
        if (!string.IsNullOrWhiteSpace(practiceCentreId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, practiceCentreId.ToLowerInvariant());
        }
    }
}
