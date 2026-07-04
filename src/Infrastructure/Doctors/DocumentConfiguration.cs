using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Doctors;

internal sealed class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("documents");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.FileUrl)
            .HasMaxLength(1000); // Legacy, no longer required

        builder.Property(d => d.FileData);

        builder.Property(d => d.ContentType)
            .HasMaxLength(100);

        builder.Property(d => d.IsActive)
            .HasDefaultValue(true);

        builder.Property(d => d.ArchivedAt);

        builder.Property(d => d.Type)
            .HasConversion<int>();

        builder.Property(d => d.Status)
            .HasConversion<int>();

        builder.Property(d => d.UploadedAt)
            .HasConversion(d => DateTime.SpecifyKind(d, DateTimeKind.Utc), v => v);
    }
}
