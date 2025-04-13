using System.Data;
using System.Reflection;
using log4net;
using Microsoft.Data.Sqlite;

namespace backend.utils;
public class DbUtils
{
    private readonly string _connectionString;
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
    private IDbConnection _instance;

    public DbUtils(string connectionString)
    {
        _connectionString = connectionString;
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
            _logger.Error("Failed to establish a database connection");
            throw new InvalidOperationException("Failed to establish a database connection", ex);
        }
    }

    private IDbConnection GetNewConnection()
    {
        _logger.Info("Trying to connect to database");
        _instance = new SqliteConnection(_connectionString);
        _instance.Open();
        return _instance;
    }
}

