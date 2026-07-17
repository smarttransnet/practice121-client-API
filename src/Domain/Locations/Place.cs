using SharedKernel;

namespace Domain.Locations;

public sealed class Place : Entity
{
    public Guid Id { get; private set; }
    public Guid MohAreaId { get; private set; }
    public string Name { get; private set; }
    public bool IsVerified { get; private set; }

    public MohArea MohArea { get; set; }

    private Place() { } // For EF Core

    public static Place Create(Guid id, Guid MohAreaId, string name, bool isVerified = false)
    {
        return new Place
        {
            Id = id,
            MohAreaId = MohAreaId,
            Name = name,
            IsVerified = isVerified
        };
    }
}
