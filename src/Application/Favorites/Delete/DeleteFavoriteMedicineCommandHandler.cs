using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Doctors;
using Domain.Favorites;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Favorites.Delete;

internal sealed class DeleteFavoriteMedicineCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext)
    : ICommandHandler<DeleteFavoriteMedicineCommand>
{
    public async Task<Result> Handle(DeleteFavoriteMedicineCommand request, CancellationToken cancellationToken)
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

        context.FavoriteMedicines.Remove(medicine);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
