using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Doctors;

internal sealed class ESignatureConfiguration : IEntityTypeConfiguration<ESignature>
{
    public void Configure(EntityTypeBuilder<ESignature> builder)
    {
        builder.ToTable("e_signatures");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.SignatureDataUrl)
            .HasColumnType("text"); // Legacy

        builder.Property(s => s.SignatureData);

        builder.Property(s => s.SignatureContentType)
            .HasMaxLength(100);

        builder.Property(s => s.IsActive)
            .HasDefaultValue(true);

        builder.Property(s => s.ArchivedAt);

        builder.Property(s => s.IpAddress)
            .IsRequired()
            .HasMaxLength(45);

        builder.Property(s => s.SignedAt)
            .HasConversion(d => DateTime.SpecifyKind(d, DateTimeKind.Utc), v => v);
    }
}
