using System.Data;
using Microsoft.Data.Sqlite;

namespace ConsoleApp1.utils;
using Microsoft.Extensions.Logging;
public class DbUtils
{
    private readonly string _connectionString;
    private readonly ILogger<DbUtils> _logger;
    private IDbConnection _instance;

    public DbUtils(string connectionString)
    {
        _connectionString = connectionString;
        
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        _logger = loggerFactory.CreateLogger<DbUtils>();
    }

    public IDbConnection GetConnection()
    {
        try
        {
            if (_instance == null || _instance.State != ConnectionState.Open)
            {
                _instance = GetNewConnection();
            }
            return _instance;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to establish a database connection");
            throw new InvalidOperationException("Failed to establish a database connection", ex);
        }
    }

    private IDbConnection GetNewConnection()
    {
        _logger.LogInformation("Trying to connect to database");
        _instance = new SqliteConnection(_connectionString);
        _instance.Open();
        return _instance;
    }
}

