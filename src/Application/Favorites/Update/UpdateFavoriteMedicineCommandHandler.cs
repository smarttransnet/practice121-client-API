using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
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
        FavoriteMedicine? medicine = await context.FavoriteMedicines
            .SingleOrDefaultAsync(f => f.Id == request.Id && f.DoctorId == userContext.UserId, cancellationToken);

        if (medicine is null)
        {
            return Result.Failure(Error.NotFound("FavoriteMedicine.NotFound", "Favorite medicine not found"));
        }

#pragma warning disable CA1862, CA1304, CA1311
        bool duplicateExists = await context.FavoriteMedicines.AsNoTracking()
            .AnyAsync(f => f.DoctorId == userContext.UserId &&
                           f.Id != request.Id &&
                           f.GenericName.ToLower() == request.GenericName.ToLower() &&
                           f.Category.ToLower() == request.Category.ToLower(),
                           cancellationToken);
#pragma warning restore CA1862, CA1304, CA1311

        if (duplicateExists)
        {
            return Result.Failure(Error.Conflict("FavoriteMedicine.Duplicate", "Another medicine with this name and category already exists in your favorites."));
        }

        medicine.GenericName = request.GenericName.Trim();
        medicine.BrandName = string.IsNullOrWhiteSpace(request.BrandName) ? null : request.BrandName.Trim();
        medicine.Category = request.Category.Trim();
        medicine.Dose = string.IsNullOrWhiteSpace(request.Dose) ? null : request.Dose.Trim();
        medicine.Frequency = string.IsNullOrWhiteSpace(request.Frequency) ? null : request.Frequency.Trim();
        medicine.Duration = string.IsNullOrWhiteSpace(request.Duration) ? null : request.Duration.Trim();
        medicine.UpdatedAt = dateTimeProvider.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
