using SharedKernel;

namespace Domain.Locations;

public sealed class MohArea : Entity
{
    public Guid Id { get; private set; }
    public Guid DistrictId { get; private set; }
    public string Name { get; private set; }

    public District District { get; set; }

    private readonly List<Place> _places = new();
    public IReadOnlyCollection<Place> Places => _places.AsReadOnly();

    private MohArea() { } // For EF Core

    public static MohArea Create(Guid id, Guid districtId, string name)
    {
        return new MohArea
        {
            Id = id,
            DistrictId = districtId,
            Name = name
        };
    }
}
