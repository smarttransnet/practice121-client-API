using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Doctors;

internal sealed class DoctorProfileConfiguration : IEntityTypeConfiguration<DoctorProfile>
{
    public void Configure(EntityTypeBuilder<DoctorProfile> builder)
    {
        builder.ToTable("doctor_profiles");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.SlmcRegNumber)
            .HasMaxLength(50);

        builder.HasIndex(p => p.SlmcRegNumber)
            .IsUnique()
            .HasFilter("slmc_reg_number IS NOT NULL");

        builder.Property(p => p.NicNumber)
            .HasMaxLength(50);

        builder.HasIndex(p => p.NicNumber)
            .IsUnique()
            .HasFilter("nic_number IS NOT NULL");

        builder.Property(p => p.MobileNumber)
            .HasMaxLength(20);

        builder.Property(p => p.Specialty)
            .HasMaxLength(100);

        builder.Property(p => p.SubSpecialty)
            .HasMaxLength(100);

        builder.Property(p => p.ProfilePictureUrl)
            .HasMaxLength(1000); // Legacy

        builder.Property(p => p.ProfilePictureData);

        builder.Property(p => p.ProfilePictureContentType)
            .HasMaxLength(100);

        builder.Property(p => p.FirstName)
            .HasMaxLength(50); // ADDED: FirstName — new column, migration required

        builder.Property(p => p.LastName)
            .HasMaxLength(50); // ADDED: LastName — new column, migration required

        builder.Property(p => p.Gender)
            .HasMaxLength(20); // ADDED: Gender — new column, migration required

        builder.Property(p => p.Bio)
            .HasMaxLength(10000); // ADDED: Bio — new column, migration required

        builder.Property(p => p.CompletionStatus)
            .HasConversion<int>();

        builder.Property(p => p.DateOfBirth)
            .HasConversion(d => d != null ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : d, v => v);

        // Native PostgreSQL Text Array
        builder.Property(p => p.Qualifications)
            .HasColumnType("text[]");

        // Relationships
        builder.HasOne(p => p.VerificationRecord)
            .WithOne(v => v.Profile)
            .HasForeignKey<VerificationRecord>(v => v.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Documents)
            .WithOne(d => d.Profile)
            .HasForeignKey(d => d.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.ESignature)
            .WithOne(s => s.Profile)
            .HasForeignKey<ESignature>(s => s.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.QualificationsList)
            .WithOne(q => q.Profile)
            .HasForeignKey(q => q.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
