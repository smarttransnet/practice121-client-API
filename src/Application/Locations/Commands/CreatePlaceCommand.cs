using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Locations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Locations.Commands;

public record CreatePlaceCommand(Guid MohAreaId, string Name) : ICommand<Guid>;

internal sealed class CreatePlaceCommandHandler(IApplicationDbContext dbContext)
    : ICommandHandler<CreatePlaceCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreatePlaceCommand request, CancellationToken cancellationToken)
    {
#pragma warning disable CA1862, CA1304, CA1311
        var existingPlace = await dbContext.Places
            .FirstOrDefaultAsync(p => p.MohAreaId == request.MohAreaId && p.Name.ToLower() == request.Name.ToLower(), cancellationToken);
#pragma warning restore CA1862, CA1304, CA1311

        if (existingPlace != null)
        {
            return existingPlace.Id;
        }

        var mohAreaExists = await dbContext.MohAreas.AnyAsync(m => m.Id == request.MohAreaId, cancellationToken);
        if (!mohAreaExists)
        {
            return Result.Failure<Guid>(new Error("Location.NotFound", "MOH Area not found.", ErrorType.NotFound));
        }

        var place = Place.Create(Guid.NewGuid(), request.MohAreaId, request.Name, isVerified: true);

        dbContext.Places.Add(place);
        await dbContext.SaveChangesAsync(cancellationToken);

        return place.Id;
    }
}
