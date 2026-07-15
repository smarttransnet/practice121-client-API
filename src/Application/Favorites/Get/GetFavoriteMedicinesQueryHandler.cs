using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Favorites.Get;

internal sealed class GetFavoriteMedicinesQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext)
    : IQueryHandler<GetFavoriteMedicinesQuery, List<FavoriteResponse>>
{
    public async Task<Result<List<FavoriteResponse>>> Handle(GetFavoriteMedicinesQuery request, CancellationToken cancellationToken)
    {
        DoctorAccount? doctor = await context.DoctorAccounts.AsNoTracking()
            .SingleOrDefaultAsync(d => d.Id == userContext.UserId, cancellationToken);

        if (doctor is null)
        {
            return Result.Failure<List<FavoriteResponse>>(Error.NotFound("DoctorAccount.NotFound", "Doctor account not found"));
        }

        List<FavoriteResponse> favorites = await context.FavoriteMedicines.AsNoTracking()
            .Where(f => f.DoctorId == doctor.Id)
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => new FavoriteResponse
            {
                Id = f.Id,
                VerifiedName = f.VerifiedName,
                Category = f.Category,
                CreatedAt = f.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return favorites;
    }
}
