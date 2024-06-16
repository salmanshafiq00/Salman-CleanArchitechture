using CleanArchitechture.Domain.Common;

namespace CleanArchitechture.Application.Common.Abstractions;

public interface IApplicationDbContext
{
    //DbSet<RefreshToken> RefreshTokens { get; }

    DbSet<Lookup> Lookups { get; }

    DbSet<LookupDetail> LookupDetails { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
