
using CleanArchitechture.Application.Common.Abstractions.Caching;
using CleanArchitechture.Domain.Common;

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
        var entity = new LookupDetail
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            Status = request.Status,
            LookupId = request.LookupId,
            ParentId = request.ParentId
        };

        dbContext.LookupDetails.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(entity.Id);
    }
}
