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

#pragma warning disable CA1862, CA1304, CA1311
        bool exists = await context.FavoriteMedicines.AsNoTracking()
            .AnyAsync(f => f.DoctorId == doctor.Id && 
                           f.VerifiedName.ToLower() == request.VerifiedName.ToLower() &&
                           f.Category.ToLower() == request.Category.ToLower(), 
                           cancellationToken);
#pragma warning restore CA1862, CA1304, CA1311
                           
        if (exists)
        {
            return Result.Failure<Guid>(Error.Conflict("FavoriteMedicine.Duplicate", "A medicine with this name and category already exists in your favorites."));
        }

        var medicine = new FavoriteMedicine
        {
            DoctorId = doctor.Id,
            VerifiedName = request.VerifiedName,
            Category = request.Category,
            CreatedAt = dateTimeProvider.UtcNow
        };

        context.FavoriteMedicines.Add(medicine);

        await context.SaveChangesAsync(cancellationToken);

        return medicine.Id;
    }
}
