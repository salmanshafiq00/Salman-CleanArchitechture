using CleanArchitechture.Application.Features.Identity.Models;
using CleanArchitechture.Domain.Common;
using CleanArchitechture.Domain.Todos;

namespace CleanArchitechture.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }

    DbSet<TodoItem> TodoItems { get; }

    DbSet<RefreshToken> RefreshTokens { get; }

    DbSet<Lookup> Lookups { get; }

    DbSet<LookupDetail> LookupDetails { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
