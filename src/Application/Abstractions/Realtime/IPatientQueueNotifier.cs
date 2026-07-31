using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Abstractions.Realtime;

public interface IPatientQueueNotifier
{
    Task NotifyQueueUpdatedAsync(Guid practiceCentreId, Guid? doctorId = null, DateTime? visitDate = null, CancellationToken cancellationToken = default);
}
