using Domain.Doctors;
using Domain.Locations;
using SharedKernel;

namespace Domain.PracticeCentres;

public sealed class PracticeCentre : Entity
{
    public Guid Id { get; private set; }
    public Guid DoctorId { get; private set; }
    public Guid PlaceId { get; private set; }
    public string ClinicName { get; private set; }
    public int? MaxPatients { get; private set; }

    public DoctorAccount Doctor { get; set; }
    public Place Place { get; set; }

    private readonly List<SessionGroup> _sessionGroups = new();
    public IReadOnlyCollection<SessionGroup> SessionGroups => _sessionGroups.AsReadOnly();

    private readonly List<Nurse> _nurses = new();
    public IReadOnlyCollection<Nurse> Nurses => _nurses.AsReadOnly();

    private PracticeCentre() { } // For EF Core

    public static PracticeCentre Create(Guid doctorId, Guid placeId, string clinicName, int? maxPatients)
    {
        return new PracticeCentre
        {
            Id = Guid.NewGuid(),
            DoctorId = doctorId,
            PlaceId = placeId,
            ClinicName = clinicName,
            MaxPatients = maxPatients
        };
    }

    public void Update(Guid placeId, string clinicName, int? maxPatients)
    {
        PlaceId = placeId;
        ClinicName = clinicName;
        MaxPatients = maxPatients;
    }

    public void AddSessionGroup(SessionGroup group)
    {
        _sessionGroups.Add(group);
    }

    public void ClearSessionGroups()
    {
        _sessionGroups.Clear();
    }

    public void AddNurse(Nurse nurse)
    {
        _nurses.Add(nurse);
    }

    public void ClearNurses()
    {
        _nurses.Clear();
    }
}
