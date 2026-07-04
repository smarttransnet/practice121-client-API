using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Doctors;

internal sealed class QualificationConfiguration : IEntityTypeConfiguration<Qualification>
{
    public void Configure(EntityTypeBuilder<Qualification> builder)
    {
        builder.ToTable("qualifications");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(q => q.CertificateData);

        builder.Property(q => q.CertificateContentType)
            .HasMaxLength(100);

        builder.Property(q => q.IsActive)
            .HasDefaultValue(true);

        builder.Property(q => q.ArchivedAt);
    }
}
