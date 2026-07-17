using Domain.Doctors;
using Domain.Favorites;
using Domain.Todos;
using Domain.Users;
using Domain.Patients;
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
    DbSet<FavoriteMedicine> FavoriteMedicines { get; }

    DbSet<PatientAccount> PatientAccounts { get; }
    DbSet<PatientDocument> PatientDocuments { get; }

    DbSet<Domain.Locations.District> Districts { get; }
    DbSet<Domain.Locations.MohArea> MohAreas { get; }
    DbSet<Domain.Locations.Place> Places { get; }
    
    DbSet<Domain.PracticeCentres.PracticeCentre> PracticeCentres { get; }
    DbSet<Domain.PracticeCentres.SessionGroup> SessionGroups { get; }
    DbSet<Domain.PracticeCentres.TimeBlock> TimeBlocks { get; }
    DbSet<Domain.PracticeCentres.Nurse> Nurses { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

