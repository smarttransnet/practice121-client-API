using Domain.PracticeCentres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

internal sealed class SessionGroupConfiguration : IEntityTypeConfiguration<SessionGroup>
{
    public void Configure(EntityTypeBuilder<SessionGroup> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.DaysOfWeekRaw)
            .IsRequired(false)
            .HasMaxLength(100);

        builder.HasMany(s => s.TimeBlocks)
            .WithOne(t => t.SessionGroup)
            .HasForeignKey(t => t.SessionGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.DaysOff)
            .WithOne(d => d.SessionGroup)
            .HasForeignKey(d => d.SessionGroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
