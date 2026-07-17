using SharedKernel;

namespace Domain.PracticeCentres;

public sealed class TimeBlock : Entity
{
    public Guid Id { get; private set; }
    public Guid SessionGroupId { get; private set; }
    
    public string Label { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }

    public SessionGroup SessionGroup { get; set; }

    private TimeBlock() { } // For EF Core

    public static TimeBlock Create(Guid sessionGroupId, string label, TimeSpan startTime, TimeSpan endTime)
    {
        return new TimeBlock
        {
            Id = Guid.NewGuid(),
            SessionGroupId = sessionGroupId,
            Label = label,
            StartTime = startTime,
            EndTime = endTime
        };
    }
}
