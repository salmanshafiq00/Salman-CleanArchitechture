using Application.Constants;
using CleanArchitechture.Application.Common.Models;
using CleanArchitechture.Domain.Common;

namespace CleanArchitechture.Application.Features.Lookups.Commands;

public record CreateLookupCommand(
    string Name,
    string Code,
    string Description,
    bool Status,
    Guid? ParentId = null) : ICacheInvalidatorCommand<Result>
{
    public string CacheKey => CacheKeys.Lookup;
}

internal sealed class CreateLookupQueryHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<CreateLookupCommand, Result>
{
    public async Task<Result> Handle(CreateLookupCommand request, CancellationToken cancellationToken)
    {
        var entity = new Lookup
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            Status = request.Status,
            ParentId = request.ParentId
        };

        dbContext.Lookups.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(CommonMessage.SAVED_SUCCESSFULLY);
    }
}
