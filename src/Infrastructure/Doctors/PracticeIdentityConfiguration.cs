using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Doctors;

internal sealed class PracticeIdentityConfiguration : IEntityTypeConfiguration<PracticeIdentity>
{
    public void Configure(EntityTypeBuilder<PracticeIdentity> builder)
    {
        builder.ToTable("practice_identities");

        builder.HasKey(pi => pi.PracticeId);

        builder.Property(pi => pi.PracticeId)
            .ValueGeneratedNever()
            .HasMaxLength(100);

        builder.Property(pi => pi.BarcodeData)
            .IsRequired()
            .HasMaxLength(1000);
    }
}
