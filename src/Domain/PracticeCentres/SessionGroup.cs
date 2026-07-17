using SharedKernel;

namespace Domain.PracticeCentres;

public sealed class SessionGroup : Entity
{
    public Guid Id { get; private set; }
    public Guid PracticeCentreId { get; private set; }
    
    // Comma-separated days like "MON,TUE,WED"
    public string DaysOfWeekRaw { get; private set; } 

    public IReadOnlyList<string> DaysOfWeek => DaysOfWeekRaw?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();

    public PracticeCentre PracticeCentre { get; set; }

    private readonly List<TimeBlock> _timeBlocks = new();
    public IReadOnlyCollection<TimeBlock> TimeBlocks => _timeBlocks.AsReadOnly();

    private SessionGroup() { } // For EF Core

    public static SessionGroup Create(Guid practiceCentreId, List<string> daysOfWeek)
    {
        return new SessionGroup
        {
            Id = Guid.NewGuid(),
            PracticeCentreId = practiceCentreId,
            DaysOfWeekRaw = string.Join(",", daysOfWeek)
        };
    }

    public void AddTimeBlock(TimeBlock block)
    {
        _timeBlocks.Add(block);
    }
}
