using System.Data;

namespace CleanArchitechture.Application.Common.Abstractions;

public interface ISqlConnectionFactory
{
    IDbConnection GetOpenConnection();

    IDbConnection CreateNewConnection();

    string GetConnectionString();
}
