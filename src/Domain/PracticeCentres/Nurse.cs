using SharedKernel;

namespace Domain.PracticeCentres;

public sealed class Nurse : Entity
{
    public Guid Id { get; private set; }
    public Guid PracticeCentreId { get; private set; }
    
    public string Name { get; private set; }
    public string PhoneNumber { get; private set; }
    public bool IsActive { get; private set; }

    public PracticeCentre PracticeCentre { get; set; }

    private Nurse() { } // For EF Core

    public static Nurse Create(Guid practiceCentreId, string name, string phoneNumber, bool isActive)
    {
        return new Nurse
        {
            Id = Guid.NewGuid(),
            PracticeCentreId = practiceCentreId,
            Name = name,
            PhoneNumber = phoneNumber,
            IsActive = isActive
        };
    }
    
    public void Update(string name, string phoneNumber, bool isActive)
    {
        Name = name;
        PhoneNumber = phoneNumber;
        IsActive = isActive;
    }
}
