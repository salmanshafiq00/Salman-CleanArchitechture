using Application.Constants;

namespace CleanArchitechture.Application.Features.Lookups.Commands;

public record DeleteLookupCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Lookup;
}

internal sealed class DeleteLookupCommandHandler(
    IApplicationDbContext dbContext,
    IPublisher publisher) 
    : ICommandHandler<DeleteLookupCommand>
{
    public async Task<Result> Handle(DeleteLookupCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Lookups.FindAsync(request.Id, cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.Lookups.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
