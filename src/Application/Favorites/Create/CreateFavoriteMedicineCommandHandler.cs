using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Doctors;
using Domain.Favorites;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Favorites.Create;

internal sealed class CreateFavoriteMedicineCommandHandler(
    IApplicationDbContext context,
    IDateTimeProvider dateTimeProvider,
    IUserContext userContext)
    : ICommandHandler<CreateFavoriteMedicineCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateFavoriteMedicineCommand request, CancellationToken cancellationToken)
    {
        DoctorAccount? doctor = await context.DoctorAccounts.AsNoTracking()
            .SingleOrDefaultAsync(d => d.Id == userContext.UserId, cancellationToken);

        if (doctor is null)
        {
            return Result.Failure<Guid>(Error.NotFound("DoctorAccount.NotFound", "Doctor account not found"));
        }

        DoctorProfile? profile = await context.DoctorProfiles.AsNoTracking()
            .FirstOrDefaultAsync(p => p.AccountId == doctor.Id, cancellationToken);

        string? doctorSpecialty = profile?.Specialty;

#pragma warning disable CA1862, CA1304, CA1311
        bool exists = !string.IsNullOrWhiteSpace(request.GenericName) && !string.IsNullOrWhiteSpace(request.Category) &&
            await context.FavoriteMedicines.AsNoTracking()
            .AnyAsync(f => f.DoctorId == doctor.Id && 
                           f.GenericName != null && f.Category != null &&
                           f.GenericName.ToLower() == request.GenericName!.ToLower() &&
                           f.Category.ToLower() == request.Category!.ToLower(), 
                           cancellationToken);
#pragma warning restore CA1862, CA1304, CA1311
                           
        if (exists)
        {
            return Result.Failure<Guid>(Error.Conflict("FavoriteMedicine.Duplicate", "A medicine with this name and category already exists in your favorites."));
        }

        var medicine = new FavoriteMedicine
        {
            DoctorId = doctor.Id,
            GenericName = string.IsNullOrWhiteSpace(request.GenericName) ? null : request.GenericName.Trim(),
            BrandName = string.IsNullOrWhiteSpace(request.BrandName) ? null : request.BrandName.Trim(),
            Category = string.IsNullOrWhiteSpace(request.Category) ? null : request.Category.Trim(),
            Dose = string.IsNullOrWhiteSpace(request.Dose) ? null : request.Dose.Trim(),
            Frequency = string.IsNullOrWhiteSpace(request.Frequency) ? null : request.Frequency.Trim(),
            Duration = string.IsNullOrWhiteSpace(request.Duration) ? null : request.Duration.Trim(),
            DoctorSpecialty = doctorSpecialty,
            CreatedAt = dateTimeProvider.UtcNow
        };

        context.FavoriteMedicines.Add(medicine);

        await context.SaveChangesAsync(cancellationToken);

        return medicine.Id;
    }
}
