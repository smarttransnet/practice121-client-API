using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Doctors;
using Domain.Favorites;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Favorites.Update;

internal sealed class UpdateFavoriteMedicineCommandHandler(
    IApplicationDbContext context,
    IDateTimeProvider dateTimeProvider,
    IUserContext userContext)
    : ICommandHandler<UpdateFavoriteMedicineCommand>
{
    public async Task<Result> Handle(UpdateFavoriteMedicineCommand request, CancellationToken cancellationToken)
    {
        DoctorAccount? doctor = await context.DoctorAccounts.AsNoTracking()
            .SingleOrDefaultAsync(d => d.Id == userContext.UserId, cancellationToken);

        if (doctor is null)
        {
            return Result.Failure(Error.NotFound("DoctorAccount.NotFound", "Doctor account not found"));
        }

        FavoriteMedicine? medicine = await context.FavoriteMedicines
            .SingleOrDefaultAsync(f => f.Id == request.Id && f.DoctorId == doctor.Id, cancellationToken);

        if (medicine is null)
        {
            return Result.Failure(Error.NotFound("FavoriteMedicine.NotFound", "Favorite medicine not found or you don't have permission."));
        }

#pragma warning disable CA1862, CA1304, CA1311
        bool exists = await context.FavoriteMedicines.AsNoTracking()
            .AnyAsync(f => f.DoctorId == doctor.Id && 
                           f.Id != request.Id &&
                           f.VerifiedName.ToLower() == request.VerifiedName.ToLower() &&
                           f.Category.ToLower() == request.Category.ToLower(), 
                           cancellationToken);
#pragma warning restore CA1862, CA1304, CA1311
                           
        if (exists)
        {
            return Result.Failure(Error.Conflict("FavoriteMedicine.Duplicate", "A medicine with this name and category already exists in your favorites."));
        }

        medicine.VerifiedName = request.VerifiedName;
        medicine.Category = request.Category;
        medicine.UpdatedAt = dateTimeProvider.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
