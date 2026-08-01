using Domain.Patients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

internal sealed class PatientOtpSessionConfiguration : IEntityTypeConfiguration<PatientOtpSession>
{
    public void Configure(EntityTypeBuilder<PatientOtpSession> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.MobileNumber).HasMaxLength(20).IsRequired();
        builder.Property(x => x.OtpHash).HasMaxLength(250).IsRequired();
        builder.Property(x => x.VerificationToken).HasMaxLength(100);

        builder.HasIndex(x => x.MobileNumber);
        builder.HasIndex(x => x.VerificationToken);
    }
}
