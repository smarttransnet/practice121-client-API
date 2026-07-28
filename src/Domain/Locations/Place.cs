using SharedKernel;

namespace Domain.Locations;

public sealed class Place : Entity
{
    public Guid Id { get; private set; }
    public Guid MohAreaId { get; private set; }
    public string Name { get; private set; }
    public bool IsVerified { get; private set; }
    public string? Address { get; private set; }
    public string? RegistrationNumber { get; private set; }

    public MohArea MohArea { get; set; }

    private Place() { } // For EF Core

    public static Place Create(Guid id, Guid mohAreaId, string name, bool isVerified = false, string? address = null, string? registrationNumber = null)
    {
        return new Place
        {
            Id = id,
            MohAreaId = mohAreaId,
            Name = name,
            IsVerified = isVerified,
            Address = address,
            RegistrationNumber = registrationNumber
        };
    }
}
