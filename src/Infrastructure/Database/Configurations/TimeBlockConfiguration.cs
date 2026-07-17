using Domain.PracticeCentres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

internal sealed class TimeBlockConfiguration : IEntityTypeConfiguration<TimeBlock>
{
    public void Configure(EntityTypeBuilder<TimeBlock> builder)
    {
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Label)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(t => t.StartTime).IsRequired();
        builder.Property(t => t.EndTime).IsRequired();
    }
}
