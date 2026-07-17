using Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

internal sealed class DistrictConfiguration : IEntityTypeConfiguration<District>
{
    public void Configure(EntityTypeBuilder<District> builder)
    {
        builder.HasKey(d => d.Id);
        
        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasMany(d => d.MohAreas)
            .WithOne(m => m.District)
            .HasForeignKey(m => m.DistrictId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
