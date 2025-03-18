using System.Data;
using System.Reflection;
using ConsoleApp1.model;
using ConsoleApp1.utils;
using log4net;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace ConsoleApp1.repository;

public class CashierDBRepository: ICashierRepository
{
    
    private DbUtils _dbUtils;
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

    public CashierDBRepository(DbUtils dbUtils, IConfigurationRoot configuration)
    {
        _dbUtils = dbUtils;
    }

    public IEnumerable<Cashier> FindAll()
    {
        _logger.Info("Attempting to retrieve all cashiers");
        var connection = _dbUtils.GetConnection();
        var cashiers = new List<Cashier>();
        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Cashiers";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Cashier c = mapReaderToEntity(reader);
                    cashiers.Add(c);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"An exception occurred while retrieving all cashiers: {ex.Message}");
            throw new InvalidOperationException("An exception occurred while retrieving all cashiers", ex);
        }
        _logger.Info("Successfully retrieved all cashiers");
        return cashiers;
    }

    public Cashier? FindById(int id)
    {
        _logger.Info($"Attempting to find cashier with ID {id}");
        var connection = _dbUtils.GetConnection();
        Cashier? cashier = null;
        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Cashiers WHERE Id = @id";
                command.Parameters.Add(new SqliteParameter("@id", id));
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    cashier = mapReaderToEntity(reader);
                }
            }
            
        }
        catch (Exception ex)
        {
            _logger.Error($"An exception occurred while retrieving cashier with id {id}: {ex.Message}");
            throw new InvalidOperationException("An exception occurred while retrieving cashier", ex);
        }
        return cashier;
    }

    public Cashier? Save(Cashier entity)
    {
        _logger.Info("Attempting to save cashier");
        var connection = _dbUtils.GetConnection();
        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Cashiers (name, password, username) VALUES (@name, @password, @username); " + "SELECT last_insert_rowid();";
                
                command.Parameters.Add(new SqliteParameter("@name", entity.Name));
                command.Parameters.Add(new SqliteParameter("@password", entity.Password));
                command.Parameters.Add(new SqliteParameter("@username", entity.Username));
                
                var generatedId = command.ExecuteScalar();
                int id = Convert.ToInt32(generatedId);
                entity.Id = id;
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"An exception occurred while adding cashier: {ex.Message}");
            throw new InvalidOperationException("An exception occurred while retrieving cashier", ex);
        }
        return entity;
    }

    public Cashier? DeleteById(int id)
    {
        Cashier? cashier = FindById(id);
        if (cashier == null)
        {
            _logger.Error($"No cashier with ID {id}");
            throw new InvalidOperationException("No cashier with ID {id}");
        }
        _logger.Info($"Attempting to delete cashier with id: {id}");
        var connection = _dbUtils.GetConnection();
        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM Cashiers WHERE Id = @id ";

                command.Parameters.Add(new SqliteParameter("@id", id));
                command.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"An exception occurred while adding cashier: {ex.Message}");
            throw new InvalidOperationException("An exception occurred while retrieving cashier", ex);
        }
        return cashier;
    }

    public Cashier? Update(Cashier entity)
    {
        _logger.Info($"Attempting to update cashier with id: :{entity.Id}");
        
        var connection = _dbUtils.GetConnection();
        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Cashiers SET name = @name, password = @password, username = @username WHERE Id = @id ";
                command.Parameters.Add(new SqliteParameter("@name", entity.Name));
                command.Parameters.Add(new SqliteParameter("@password", entity.Password));
                command.Parameters.Add(new SqliteParameter("@username", entity.Username));
                command.Parameters.Add(new SqliteParameter("@id", entity.Id));
                command.ExecuteNonQuery();
            }
            return entity;
        }
        catch (Exception ex)
        {
            _logger.Error($"An exception occurred while updating cashier with id: {entity.Id}: {ex.Message}");
            throw new InvalidOperationException("An exception occurred while updating cashier", ex);
        }
    }

    private Cashier mapReaderToEntity(IDataReader reader)
    {
        int id = reader.GetInt32(0);
        string name = reader.GetString(1);
        string password = reader.GetString(2);
        string username = reader.GetString(3);
        return new Cashier(id, name, password, username);
    }
}