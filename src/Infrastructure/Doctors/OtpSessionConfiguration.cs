using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Doctors;

internal sealed class OtpSessionConfiguration : IEntityTypeConfiguration<OtpSession>
{
    public void Configure(EntityTypeBuilder<OtpSession> builder)
    {
        builder.ToTable("otp_sessions");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.OtpHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(o => o.Channel)
            .HasConversion<int>();

        builder.Property(o => o.ExpiresAt)
            .HasConversion(d => DateTime.SpecifyKind(d, DateTimeKind.Utc), v => v);

        builder.Property(o => o.Verified)
            .IsRequired();

        builder.Property(o => o.Attempts)
            .IsRequired();
    }
}
