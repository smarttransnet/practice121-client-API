using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.PatientQueue;
using Infrastructure.Database;

namespace Infrastructure.Database.Configurations;

public class PatientQueueTicketConfiguration : IEntityTypeConfiguration<PatientQueueTicket>
{
    public void Configure(EntityTypeBuilder<PatientQueueTicket> builder)
    {
        builder.ToTable("PatientQueueTicket", Schemas.Default);
        builder.HasKey(q => q.Id);
        builder.Property(q => q.QueueNumber).IsRequired();
        builder.Property(q => q.QueueOrder).IsRequired();
        builder.Property(q => q.PatientMobile).IsRequired();
        builder.Property(q => q.DoctorId).IsRequired();
        builder.Property(q => q.PracticeCentreId).IsRequired();
        builder.Property(q => q.VisitDate).HasColumnType("date").IsRequired();
        builder.Property(q => q.Status).IsRequired();
        builder.Property(q => q.Priority).IsRequired();
        builder.Property(q => q.CreatedAt).HasDefaultValueSql("now()");
        builder.HasIndex(q => new { q.PracticeCentreId, q.DoctorId, q.VisitDate, q.Status });
    }
}
