using CleanArchitechture.Application.Features.Admin.AppPages.Queries;
using CleanArchitechture.Domain.Admin;

namespace CleanArchitechture.Application.Features.Admin.AppPages.Commands;

public record InsertAppPageCommand : AppPageModel, IRequest<Guid>;

internal sealed class InsertAppPageCommandHandler(IApplicationDbContext dbContext, IMapper mapper) 
    : IRequestHandler<InsertAppPageCommand, Guid>
{
    public async Task<Guid> Handle(InsertAppPageCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.AppPages
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(ap => ap.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            entity = mapper.Map<AppPage>(request);
            await dbContext.AppPages.AddAsync(entity, cancellationToken);
        }
        else
        {
            mapper.Map(request, entity);
            dbContext.AppPages.Update(entity);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
