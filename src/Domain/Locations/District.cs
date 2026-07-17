using SharedKernel;

namespace Domain.Locations;

public sealed class District : Entity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    
    private readonly List<MohArea> _mohAreas = new();
    public IReadOnlyCollection<MohArea> MohAreas => _mohAreas.AsReadOnly();

    private District() { } // For EF Core

    public static District Create(Guid id, string name)
    {
        return new District
        {
            Id = id,
            Name = name
        };
    }
}
