using Domain.PracticeCentres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

internal sealed class PracticeCentreConfiguration : IEntityTypeConfiguration<PracticeCentre>
{
    public void Configure(EntityTypeBuilder<PracticeCentre> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.ClinicName)
            .IsRequired()
            .HasMaxLength(250);

        builder.HasMany(p => p.SessionGroups)
            .WithOne(s => s.PracticeCentre)
            .HasForeignKey(s => s.PracticeCentreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Nurses)
            .WithOne(n => n.PracticeCentre)
            .HasForeignKey(n => n.PracticeCentreId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(p => p.Doctor)
            .WithMany()
            .HasForeignKey(p => p.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(p => p.Place)
            .WithMany()
            .HasForeignKey(p => p.PlaceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
