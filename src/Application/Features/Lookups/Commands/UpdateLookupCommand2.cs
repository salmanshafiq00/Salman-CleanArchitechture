using Application.Constants;
using CleanArchitechture.Application.Common.Events;
using CleanArchitechture.Application.Common.Models;
using CleanArchitechture.Domain.Common.DomainEvents;

namespace CleanArchitechture.Application.Features.Lookups.Commands;

public record UpdateLookupCommand2(
    Guid Id,
    string Name,
    string Code,
    string Description,
    bool Status,
    Guid? ParentId) : ICommand<Result>;

internal sealed class UpdateLookupCommandHandler2(
    IApplicationDbContext dbContext,
    IPublisher publisher) 
    : ICommandHandler<UpdateLookupCommand2, Result>
{
    public async Task<Result> Handle(UpdateLookupCommand2 request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Lookups.FindAsync(request.Id, cancellationToken);

        //Guard.Against.NotFound(request.Id, entity);

        if (entity is null)
        {
            return Result.NotFound(ErrorMessages.EntityNotFound);
        }

        bool oldStatus = entity.Status;

        entity.Name = request.Name;
        entity.Code = request.Code;
        entity.Description = request.Description;
        entity.Status = request.Status;
        entity.ParentId = request.ParentId;

        entity.AddDomainEvent(new LookupUpdatedEvent(entity));

        await dbContext.SaveChangesAsync(cancellationToken);

        await publisher.Publish(
new CacheInvalidationEvent { CacheKey = CacheKeys.Lookup });

        if (oldStatus != request.Status)
        {
            await publisher.Publish(
    new CacheInvalidationEvent { CacheKey = CacheKeys.LookupDetail });
        }

        return Result.Success(CommonMessage.SAVED_SUCCESSFULLY);
    }
}
