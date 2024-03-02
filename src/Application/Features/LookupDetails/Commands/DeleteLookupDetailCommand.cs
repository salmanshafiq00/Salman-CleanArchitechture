using Application.Constants;
using CleanArchitechture.Application.Common.Abstractions;
using CleanArchitechture.Application.Common.Abstractions.Messaging;

namespace CleanArchitechture.Application.Features.LookupDetails.Commands;

public record DeleteLookupDetailCommand(Guid Id) : ICommand
{
    public string CacheKey => CacheKeys.LookupDetail;
}

internal sealed class DeleteLookupDetailCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<DeleteLookupDetailCommand>
{
    public async Task<Result> Handle(DeleteLookupDetailCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.LookupDetails.FindAsync(request.Id, cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.LookupDetails.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
        //return Result.Success(CommonMessage.UPDATED_SUCCESSFULLY);
    }
}
