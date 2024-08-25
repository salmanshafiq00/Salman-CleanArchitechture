using CleanArchitechture.Application.Common.Abstractions.Caching;
using CleanArchitechture.Domain.Common;
using Mapster;

namespace CleanArchitechture.Application.Features.LookupDetails.Commands;

public record CreateLookupDetailCommand(
    string Name,
    string Code,
    string Description,
    bool Status,
    Guid LookupId,
    Guid? ParentId = null) : ICacheInvalidatorCommand<Guid>
{
   public string CacheKey => CacheKeys.LookupDetail;
}

internal sealed class CreateLookupDetailQueryHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<CreateLookupDetailCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateLookupDetailCommand request, CancellationToken cancellationToken)
    {
        var entity = request.Adapt<LookupDetail>();

        dbContext.LookupDetails.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(entity.Id);
    }
}
