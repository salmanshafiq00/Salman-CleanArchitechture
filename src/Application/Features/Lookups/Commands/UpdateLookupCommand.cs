using Application.Constants;
using CleanArchitechture.Application.Common.Events;
using CleanArchitechture.Application.Common.Models;
using CleanArchitechture.Domain.Common.DomainEvents;

namespace CleanArchitechture.Application.Features.Lookups.Commands;

public record UpdateLookupCommand(
    Guid Id,
    string Name,
    string Code,
    string Description,
    bool Status,
    Guid? ParentId) : ICacheInvalidatorCommand<Result>
{
    public string CacheKey => CacheKeys.Lookup;
}

internal sealed class UpdateLookupCommandHandler(
    IApplicationDbContext dbContext,
    IPublisher publisher)
    : ICommandHandler<UpdateLookupCommand, Result>
{
    public async Task<Result> Handle(UpdateLookupCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Lookups.FindAsync(request.Id, cancellationToken);

        if (entity is null) return Result.NotFound(ErrorMessages.EntityNotFound);

        bool oldStatus = entity.Status;

        entity.Name = request.Name;
        entity.Code = request.Code;
        entity.Description = request.Description;
        entity.Status = request.Status;
        entity.ParentId = request.ParentId;

        entity.AddDomainEvent(new LookupUpdatedEvent(entity));

        await dbContext.SaveChangesAsync(cancellationToken);

        if (oldStatus != request.Status)
        {
            await publisher.Publish(
    new CacheInvalidationEvent { CacheKey = CacheKeys.LookupDetail });
        }

        return Result.Success(CommonMessage.UPDATED_SUCCESSFULLY);
    }
}
