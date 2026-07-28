using Domain.Doctors;
using Domain.Favorites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Favorites;

internal sealed class FavoriteMedicineConfiguration : IEntityTypeConfiguration<FavoriteMedicine>
{
    public void Configure(EntityTypeBuilder<FavoriteMedicine> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.GenericName)
            .HasMaxLength(200);

        builder.Property(f => f.BrandName)
            .HasMaxLength(200);

        builder.Property(f => f.Category)
            .HasMaxLength(100);

        builder.Property(f => f.Dose)
            .HasMaxLength(100);

        builder.Property(f => f.Frequency)
            .HasMaxLength(100);

        builder.Property(f => f.Duration)
            .HasMaxLength(100);

        builder.Property(f => f.DoctorSpecialty)
            .HasMaxLength(200);

        builder.HasIndex(f => f.DoctorSpecialty);

        builder.HasOne<DoctorAccount>()
            .WithMany()
            .HasForeignKey(f => f.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
