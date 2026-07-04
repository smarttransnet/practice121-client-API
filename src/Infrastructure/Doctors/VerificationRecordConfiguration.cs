using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Doctors;

internal sealed class VerificationRecordConfiguration : IEntityTypeConfiguration<VerificationRecord>
{
    public void Configure(EntityTypeBuilder<VerificationRecord> builder)
    {
        builder.ToTable("verification_records");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.VerifiedAt)
            .HasConversion(d => d != null ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : d, v => v);

        // Verification fields
        builder.Property(v => v.SlmcVerified).IsRequired();
        builder.Property(v => v.QualificationsVerified).IsRequired();
        builder.Property(v => v.DocumentsVerified).IsRequired();
        builder.Property(v => v.BadgeAwarded).IsRequired();
    }
}
