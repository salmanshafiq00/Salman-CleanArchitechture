using System.Data;
using System.Data.SqlClient;
using CleanArchitechture.Application.Common.Abstractions;

namespace WebApi.Infrastructure.Persistence;

internal sealed class SqlConnectionFactory(string connectionString) 
    : ISqlConnectionFactory, IDisposable
{
    private IDbConnection _connection;
    private readonly string _connectionString = connectionString;

    public IDbConnection CreateNewConnection()
    {
        var connection = new SqlConnection(_connectionString);
        connection.Open();

        return connection;
    }

    public void Dispose()
    {
        if(_connection is not null && _connection.State is ConnectionState.Open)
        {
            _connection.Dispose();
        }
    }

    public string GetConnectionString()
    {
        return _connectionString;
    }

    public IDbConnection GetOpenConnection()
    {
        if(_connection is null || _connection.State is not ConnectionState.Open)
        {
            _connection = new SqlConnection(_connectionString);
            _connection.Open();
        }
        return _connection;
    }
}
