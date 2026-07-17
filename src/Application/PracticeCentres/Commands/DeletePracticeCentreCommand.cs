using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.PracticeCentres.Commands;

public record DeletePracticeCentreCommand(Guid Id, Guid DoctorId) : ICommand;

internal sealed class DeletePracticeCentreCommandHandler(IApplicationDbContext dbContext)
    : ICommandHandler<DeletePracticeCentreCommand>
{
    public async Task<Result> Handle(DeletePracticeCentreCommand request, CancellationToken cancellationToken)
    {
        var centre = await dbContext.PracticeCentres
            .Include(pc => pc.SessionGroups)
                .ThenInclude(sg => sg.TimeBlocks)
            .Include(pc => pc.Nurses)
            .FirstOrDefaultAsync(pc => pc.Id == request.Id && pc.DoctorId == request.DoctorId, cancellationToken);

        if (centre == null)
        {
            return Result.Failure(new Error("PracticeCentre.NotFound", "The specified Practice Centre was not found or you do not have permission to delete it.", ErrorType.NotFound));
        }

        dbContext.SessionGroups.RemoveRange(centre.SessionGroups);
        dbContext.Nurses.RemoveRange(centre.Nurses);
        dbContext.PracticeCentres.Remove(centre);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
