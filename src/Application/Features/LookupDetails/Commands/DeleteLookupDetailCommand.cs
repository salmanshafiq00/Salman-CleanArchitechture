using Application.Constants;
using CleanArchitechture.Application.Common.Events;
using CleanArchitechture.Application.Common.Models;

namespace CleanArchitechture.Application.Features.LookupDetails.Commands;

public record DeleteLookupDetailCommand(Guid Id) : ICommand<Result>;

internal sealed class DeleteLookupDetailCommandHandler(
    IApplicationDbContext dbContext,
    IPublisher publisher) 
    : ICommandHandler<DeleteLookupDetailCommand, Result>
{
    public async Task<Result> Handle(DeleteLookupDetailCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.LookupDetails.FindAsync(request.Id, cancellationToken);

        if (entity is null) return Result.NotFound(ErrorMessages.NotFound);

        dbContext.LookupDetails.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        await publisher.Publish(
    new CacheInvalidationEvent { CacheKey = CacheKeys.LookupDetail });

        return Result.Success(CommonMessage.UPDATED_SUCCESSFULLY);
    }
}
