using Domain.Doctors;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<TodoItem> TodoItems { get; }
    DbSet<DoctorAccount> DoctorAccounts { get; }
    DbSet<DoctorProfile> DoctorProfiles { get; }
    DbSet<VerificationRecord> VerificationRecords { get; }
    DbSet<PracticeIdentity> PracticeIdentities { get; }
    DbSet<Document> Documents { get; }
    DbSet<OtpSession> OtpSessions { get; }
    DbSet<ESignature> ESignatures { get; }
    DbSet<Qualification> Qualifications { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

