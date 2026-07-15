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

        builder.Property(f => f.VerifiedName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(f => f.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne<DoctorAccount>()
            .WithMany()
            .HasForeignKey(f => f.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
