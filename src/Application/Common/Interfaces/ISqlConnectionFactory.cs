using System.Data;

namespace CleanArchitechture.Application.Common.Interfaces;

public interface ISqlConnectionFactory
{
    IDbConnection GetOpenConnection();

    IDbConnection CreateNewConnection();

    string GetConnectionString();
}
