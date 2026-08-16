using SharedKernel;

namespace Domain.PracticeCentres;

public sealed class SessionGroup : Entity
{
    public Guid Id { get; private set; }
    public Guid PracticeCentreId { get; private set; }
    
    // Comma-separated days like "MON,TUE,WED" for recurring sessions
    public string? DaysOfWeekRaw { get; private set; } 

    public IReadOnlyList<string> DaysOfWeek => DaysOfWeekRaw?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();

    // Specific Date for ad-hoc sessions (e.g. weekend opt-ins)
    public DateOnly? SpecificDate { get; private set; }

    public PracticeCentre PracticeCentre { get; set; }

    private readonly List<TimeBlock> _timeBlocks = new();
    public IReadOnlyCollection<TimeBlock> TimeBlocks => _timeBlocks.AsReadOnly();

    private readonly List<SessionGroupDayOff> _daysOff = new();
    public IReadOnlyCollection<SessionGroupDayOff> DaysOff => _daysOff.AsReadOnly();

    private SessionGroup() { } // For EF Core

    public static SessionGroup Create(Guid practiceCentreId, List<string> daysOfWeek, DateOnly? specificDate = null)
    {
        return new SessionGroup
        {
            Id = Guid.NewGuid(),
            PracticeCentreId = practiceCentreId,
            DaysOfWeekRaw = daysOfWeek != null && daysOfWeek.Any() ? string.Join(",", daysOfWeek) : null,
            SpecificDate = specificDate
        };
    }

    public void AddTimeBlock(TimeBlock block)
    {
        _timeBlocks.Add(block);
    }

    public void AddDayOff(SessionGroupDayOff dayOff)
    {
        _daysOff.Add(dayOff);
    }

    public void ClearDaysOff()
    {
        _daysOff.Clear();
    }
}
