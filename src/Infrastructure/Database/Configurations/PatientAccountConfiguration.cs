using Domain.Patients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

internal sealed class PatientAccountConfiguration : IEntityTypeConfiguration<PatientAccount>
{
    public void Configure(EntityTypeBuilder<PatientAccount> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.NicNumber)
            .IsUnique()
            .HasFilter("nic_number IS NOT NULL AND nic_number <> ''");

        builder.Property(x => x.NicNumber).HasMaxLength(20).IsRequired(false);
        builder.Property(x => x.FirstName).HasMaxLength(100);
        builder.Property(x => x.LastName).HasMaxLength(100);
        builder.Property(x => x.MobileNumber).HasMaxLength(20);
        
        builder.HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Documents)
            .WithOne(d => d.PatientAccount)
            .HasForeignKey(d => d.PatientAccountId);
    }
}
