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

        if (request.Medicines == null || !request.Medicines.Any())
        {
            return 0; // nothing to add
        }

        var medicinesToAdd = request.Medicines.Select(m => new FavoriteMedicine
        {
            DoctorId = doctor.Id,
            VerifiedName = m.VerifiedName,
            Category = m.Category,
            CreatedAt = dateTimeProvider.UtcNow
        }).ToList();

        context.FavoriteMedicines.AddRange(medicinesToAdd);

        await context.SaveChangesAsync(cancellationToken);

        return medicinesToAdd.Count;
    }
}
