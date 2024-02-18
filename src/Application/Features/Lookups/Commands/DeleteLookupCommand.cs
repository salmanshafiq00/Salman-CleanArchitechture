using Application.Constants;
using CleanArchitechture.Application.Common.Models;

namespace CleanArchitechture.Application.Features.Lookups.Commands;

public record DeleteLookupCommand(Guid Id) : ICacheInvalidatorCommand<Result>
{
    public string CacheKey => CacheKeys.Lookup;
}

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

        return Result.Success(CommonMessage.DELETED_SUCCESSFULLY);
    }
}
