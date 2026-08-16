using SharedKernel;

namespace Domain.PracticeCentres;

public sealed class SessionGroupDayOff : Entity
{
    public Guid Id { get; private set; }
    public Guid SessionGroupId { get; private set; }
    public DateOnly Date { get; private set; }

    public SessionGroup SessionGroup { get; set; }

    private SessionGroupDayOff() { }

    public static SessionGroupDayOff Create(Guid sessionGroupId, DateOnly date)
    {
        return new SessionGroupDayOff
        {
            Id = Guid.NewGuid(),
            SessionGroupId = sessionGroupId,
            Date = date
        };
    }
}
