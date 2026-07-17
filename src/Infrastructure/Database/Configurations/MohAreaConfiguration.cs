using Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

internal sealed class MohAreaConfiguration : IEntityTypeConfiguration<MohArea>
{
    public void Configure(EntityTypeBuilder<MohArea> builder)
    {
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasMany(m => m.Places)
            .WithOne(p => p.MohArea)
            .HasForeignKey(p => p.MohAreaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
