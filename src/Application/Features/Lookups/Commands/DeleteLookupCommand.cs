using Application.Constants;
using CleanArchitechture.Application.Common.Events;
using CleanArchitechture.Application.Common.Models;

namespace CleanArchitechture.Application.Features.Lookups.Commands;

public record DeleteLookupCommand(Guid Id) : ICommand<Result>;

internal sealed class DeleteLookupCommandHandler(
    IApplicationDbContext dbContext,
    IPublisher publisher) 
    : ICommandHandler<DeleteLookupCommand, Result>
{
    public async Task<Result> Handle(DeleteLookupCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Lookups.FindAsync(request.Id, cancellationToken);

        if (entity is null) return Result.NotFound(ErrorMessages.EntityNotFound);

        dbContext.Lookups.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        await publisher.Publish(
    new CacheInvalidationEvent { CacheKey = CacheKeys.Lookup });

        return Result.Success(CommonMessage.DELETED_SUCCESSFULLY);
    }
}
