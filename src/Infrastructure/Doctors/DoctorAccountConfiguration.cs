using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Doctors;

internal sealed class DoctorAccountConfiguration : IEntityTypeConfiguration<DoctorAccount>
{
    public void Configure(EntityTypeBuilder<DoctorAccount> builder)
    {
        builder.ToTable("doctor_accounts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(a => a.Email)
            .IsUnique();

        builder.Property(a => a.GoogleSubId)
            .HasMaxLength(255);

        builder.HasIndex(a => a.GoogleSubId)
            .IsUnique()
            .HasFilter("google_sub_id IS NOT NULL");

        builder.Property(a => a.PasswordHash)
            .HasMaxLength(255);

        builder.Property(a => a.AuthProvider)
            .HasConversion<int>();

        builder.Property(a => a.Status)
            .HasConversion<int>();

        builder.Property(a => a.CreatedAt)
            .HasConversion(d => DateTime.SpecifyKind(d, DateTimeKind.Utc), v => v);

        builder.Property(a => a.RefreshToken)
            .HasMaxLength(500);

        builder.Property(a => a.RefreshTokenExpiry)
            .HasConversion(d => d != null ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : d, v => v);

        // Relationships
        builder.HasOne(a => a.Profile)
            .WithOne(p => p.Account)
            .HasForeignKey<DoctorProfile>(p => p.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.PracticeIdentity)
            .WithOne(pi => pi.Account)
            .HasForeignKey<PracticeIdentity>(pi => pi.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.OtpSessions)
            .WithOne(o => o.Account)
            .HasForeignKey(o => o.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
