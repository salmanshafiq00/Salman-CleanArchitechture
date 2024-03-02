using Application.Constants;
using CleanArchitechture.Application.Common.Abstractions;
using CleanArchitechture.Application.Common.Abstractions.Messaging;

namespace CleanArchitechture.Application.Features.LookupDetails.Commands;

public record UpdateLookupDetailCommand(
    Guid Id,
    string Name,
    string Code,
    string Description,
    bool Status,
    Guid LookupId,
    Guid? ParentId) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.LookupDetail;
}

internal sealed class UpdateLookupDetailCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<UpdateLookupDetailCommand>
{
    public async Task<Result> Handle(UpdateLookupDetailCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.LookupDetails.FindAsync(request.Id, cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        entity.Name = request.Name;
        entity.Code = request.Code;
        entity.Description = request.Description;
        entity.Status = request.Status;
        entity.LookupId = request.LookupId;
        entity.ParentId = request.ParentId;

        await dbContext.SaveChangesAsync(cancellationToken);

        //return Result.Success(CommonMessage.UPDATED_SUCCESSFULLY);
        return Result.Success();
    }
}
