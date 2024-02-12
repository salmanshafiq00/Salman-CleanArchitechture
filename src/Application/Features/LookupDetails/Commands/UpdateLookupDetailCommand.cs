using Application.Constants;
using CleanArchitechture.Application.Common.Events;
using CleanArchitechture.Application.Common.Models;

namespace CleanArchitechture.Application.Features.LookupDetails.Commands;

public record UpdateLookupDetailCommand(
    Guid Id,
    string Name,
    string Code,
    string Description,
    bool Status,
    Guid LookupId,
    Guid? ParentId) : ICommand<Result>;

internal sealed class UpdateLookupDetailCommandHandler(
    IApplicationDbContext dbContext,
    IPublisher publisher) 
    : ICommandHandler<UpdateLookupDetailCommand, Result>
{
    public async Task<Result> Handle(UpdateLookupDetailCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.LookupDetails.FindAsync(request.Id, cancellationToken);

        if (entity is null) return Result.NotFound(ErrorMessages.NotFound);

        entity.Name = request.Name;
        entity.Code = request.Code;
        entity.Description = request.Description;
        entity.Status = request.Status;
        entity.LookupId = request.LookupId;
        entity.ParentId = request.ParentId;

        await dbContext.SaveChangesAsync(cancellationToken);

        await publisher.Publish(
            new CacheInvalidationEvent { CacheKey = CacheKeys.LookupDetail });

        return Result.Success(CommonMessage.UPDATED_SUCCESSFULLY);
    }
}
