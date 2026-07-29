using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Doctors;
using Domain.Favorites;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Favorites.BulkCreate;

internal sealed class BulkCreateFavoriteMedicinesCommandHandler(
    IApplicationDbContext context,
    IDateTimeProvider dateTimeProvider,
    IUserContext userContext)
    : ICommandHandler<BulkCreateFavoriteMedicinesCommand, int>
{
    public async Task<Result<int>> Handle(BulkCreateFavoriteMedicinesCommand request, CancellationToken cancellationToken)
    {
        DoctorAccount? doctor = await context.DoctorAccounts.AsNoTracking()
            .SingleOrDefaultAsync(d => d.Id == userContext.UserId, cancellationToken);

        if (doctor is null)
        {
            return Result.Failure<int>(Error.NotFound("DoctorAccount.NotFound", "Doctor account not found"));
        }

        if (request.Medicines == null || request.Medicines.Count == 0)
        {
            return 0;
        }

        DoctorProfile? profile = await context.DoctorProfiles.AsNoTracking()
            .FirstOrDefaultAsync(p => p.AccountId == doctor.Id, cancellationToken);

        string? doctorSpecialty = profile?.Specialty;

        var medicinesToAdd = request.Medicines.Select(m => new FavoriteMedicine
        {
            DoctorId = doctor.Id,
            GenericName = string.IsNullOrWhiteSpace(m.GenericName) ? null : m.GenericName.Trim(),
            BrandName = string.IsNullOrWhiteSpace(m.BrandName) ? null : m.BrandName.Trim(),
            Category = string.IsNullOrWhiteSpace(m.Category) ? null : m.Category.Trim(),
            Dose = string.IsNullOrWhiteSpace(m.Dose) ? null : m.Dose.Trim(),
            Frequency = string.IsNullOrWhiteSpace(m.Frequency) ? null : m.Frequency.Trim(),
            Duration = string.IsNullOrWhiteSpace(m.Duration) ? null : m.Duration.Trim(),
            DoctorSpecialty = doctorSpecialty,
            CreatedAt = dateTimeProvider.UtcNow
        }).ToList();

        context.FavoriteMedicines.AddRange(medicinesToAdd);

        await context.SaveChangesAsync(cancellationToken);

        return medicinesToAdd.Count;
    }
}
