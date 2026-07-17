using Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

internal sealed class PlaceConfiguration : IEntityTypeConfiguration<Place>
{
    public void Configure(EntityTypeBuilder<Place> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(250);
            
        builder.Property(p => p.IsVerified)
            .IsRequired()
            .HasDefaultValue(false);
            
        // Assuming Places must be unique within an MohArea to avoid duplicates
        builder.HasIndex(p => new { p.MohAreaId, p.Name }).IsUnique();
    }
}
