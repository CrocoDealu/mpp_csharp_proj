using System.Data;
using System.Text;
using ConsoleApp1.dto;
using ConsoleApp1.model;
using ConsoleApp1.utils;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace ConsoleApp1.repository;

public class TicketDBRepository : ITicketRepository
{
    private readonly DbUtils _dbUtils;
    private readonly ILogger<TicketDBRepository> _logger;
    private readonly IGameRepository _gameRepository;
    private readonly ICashierRepository _cashierRepository;

    public TicketDBRepository(DbUtils dbUtils, IGameRepository gameRepository, ICashierRepository cashierRepository)
    {
        _dbUtils = dbUtils;
        _gameRepository = gameRepository;
        _cashierRepository = cashierRepository;

        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });
        _logger = loggerFactory.CreateLogger<TicketDBRepository>();
    }

    public Ticket? FindById(int id)
    {
        _logger.LogInformation("Attempting to find ticket with ID {id}", id);
        var connection = _dbUtils.GetConnection();
        Ticket? ticket = null;
        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Tickets WHERE Id = @id";
                command.Parameters.Add(new SqliteParameter("@id", id));
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    ticket = MapReaderToEntity(reader);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"An exception occurred while retrieving ticket with id {id}: {ex.Message}");
            throw new InvalidOperationException("An exception occurred while retrieving the ticket", ex);
        }
        return ticket;
    }

    public IEnumerable<Ticket> FindAll()
    {
        _logger.LogInformation("Attempting to retrieve all tickets");
        var connection = _dbUtils.GetConnection();
        var tickets = new List<Ticket>();
        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Tickets";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Ticket ticket = MapReaderToEntity(reader);
                    tickets.Add(ticket);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"An exception occurred while retrieving all tickets: {ex.Message}");
            throw new InvalidOperationException("An exception occurred while retrieving all tickets", ex);
        }
        _logger.LogInformation("Successfully retrieved all tickets");
        return tickets;
    }

    public Ticket Save(Ticket entity)
    {
        _logger.LogInformation("Attempting to save ticket");
        var connection = _dbUtils.GetConnection();
        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Tickets (game_id, customer_name, customer_address, cashier_id, no_of_seats, price) VALUES (@gameId, @customerName, @customerAddress, @cashierId, @noOfSeats, @price); SELECT last_insert_rowid();";

                command.Parameters.Add(new SqliteParameter("@gameId", entity.Game.Id));
                command.Parameters.Add(new SqliteParameter("@customerName", entity.CustomerName));
                command.Parameters.Add(new SqliteParameter("@customerAddress", entity.CustomerAddress));
                command.Parameters.Add(new SqliteParameter("@cashierId", entity.Seller.Id));
                command.Parameters.Add(new SqliteParameter("@noOfSeats", entity.NoOfSeats));
                command.Parameters.Add(new SqliteParameter("@price", entity.Price));

                var generatedId = command.ExecuteScalar();
                int id = Convert.ToInt32(generatedId);
                entity.Id = id;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"An exception occurred while saving ticket: {ex.Message}");
            throw new InvalidOperationException("An exception occurred while saving the ticket", ex);
        }
        return entity;
    }

    public Ticket? DeleteById(int id)
    {
        Ticket? ticket = FindById(id);
        if (ticket == null)
        {
            _logger.LogInformation("No ticket found with ID {id}", id);
            throw new InvalidOperationException($"No ticket found with ID {id}");
        }
        _logger.LogInformation("Attempting to delete ticket with ID {id}", id);
        var connection = _dbUtils.GetConnection();
        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM Tickets WHERE Id = @id";
                command.Parameters.Add(new SqliteParameter("@id", id));
                command.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"An exception occurred while deleting ticket with id {id}: {ex.Message}");
            throw new InvalidOperationException("An exception occurred while deleting the ticket", ex);
        }
        return ticket;
    }

    public Ticket Update(Ticket entity)
    {
        _logger.LogInformation("Attempting to update ticket with ID {id}", entity.Id);
        var connection = _dbUtils.GetConnection();
        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Tickets SET game_id = @gameId, customer_name = @customerName, customer_address = @customerAddress, cashier_id = @cashierId, no_of_seats = @noOfSeats, price = @price WHERE Id = @id";

                command.Parameters.Add(new SqliteParameter("@gameId", entity.Game.Id));
                command.Parameters.Add(new SqliteParameter("@customerName", entity.CustomerName));
                command.Parameters.Add(new SqliteParameter("@customerAddress", entity.CustomerAddress));
                command.Parameters.Add(new SqliteParameter("@cashierId", entity.Seller.Id));
                command.Parameters.Add(new SqliteParameter("@noOfSeats", entity.NoOfSeats));
                command.Parameters.Add(new SqliteParameter("@price", entity.Price));
                command.Parameters.Add(new SqliteParameter("@id", entity.Id));

                command.ExecuteNonQuery();
            }
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An exception occurred while updating ticket with id {entity.Id}: {ex.Message}");
            throw new InvalidOperationException("An exception occurred while updating the ticket", ex);
        }
    }

    public IEnumerable<Ticket> GetTicketsSoldForGame(Game game)
    {
        _logger.LogInformation("Attempting to retrieve tickets sold for game with ID {gameId}", game.Id);
        var connection = _dbUtils.GetConnection();
        var tickets = new List<Ticket>();
        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Tickets WHERE game_id = @gameId";
                command.Parameters.Add(new SqliteParameter("@gameId", game.Id));
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Ticket ticket = MapReaderToEntity(reader);
                    tickets.Add(ticket);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"An exception occurred while retrieving tickets for game with id {game.Id}: {ex.Message}");
            throw new InvalidOperationException("An exception occurred while retrieving tickets for the game", ex);
        }
        _logger.LogInformation("Successfully retrieved tickets for game with ID {gameId}", game.Id);
        return tickets;
    }

    public IEnumerable<Ticket> GetTicketsForClient(ClientFilterDTO filter)
    {
        _logger.LogInformation($"Attempting to get all tickets for client that has name: {filter.ClientName} and address: {filter.ClientAddress}");
        var connection = _dbUtils.GetConnection();
        var tickets = new List<Ticket>();
        try
        {
            using (var command = connection.CreateCommand())
            {
                string query = "SELECT * FROM Tickets ";
                query = ProcessFilter(query, filter);
                command.CommandText = query;
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Ticket ticket = MapReaderToEntity(reader);
                    tickets.Add(ticket);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"An exception occured when getting all tickets for client {ex.Message}");
            throw new InvalidOperationException("An exception occurred when getting all tickets for client", ex);
        }
        return tickets;
    }

    private string ProcessFilter(string sql, ClientFilterDTO filter)
    {
        StringBuilder modified = new StringBuilder(sql);
        if (filter != null)
        {
            modified.Append(" WHERE ");
            List<string> paramsList = new List<string>();
            List<string> valuesList = new List<string>();

            if (!string.IsNullOrEmpty(filter.ClientAddress))
            {
                paramsList.Add("customer_address = ");
                valuesList.Add(filter.ClientAddress);
            }
            if (!string.IsNullOrEmpty(filter.ClientName))
            {
                paramsList.Add("customer_name = ");
                valuesList.Add(filter.ClientName);
            }

            for (int i = 0; i < paramsList.Count; i++)
            {
                if (i != paramsList.Count - 1)
                {
                    modified.Append(paramsList[i])
                        .Append("'").Append(valuesList[i]).Append("'")
                        .Append(" AND ");
                }
                else
                {
                    modified.Append(paramsList[i])
                        .Append("'").Append(valuesList[i]).Append("'");
                }
            }
        }
        return modified.ToString();
    }

    private Ticket MapReaderToEntity(IDataReader reader)
    {
        int id = reader.GetInt32(0);
        int gameId = reader.GetInt32(1);
        string customerName = reader.GetString(2);
        string customerAddress = reader.GetString(3);
        int cashierId = reader.GetInt32(4);
        int noOfSeats = reader.GetInt32(5);
        float price = reader.GetFloat(6);

        var game = _gameRepository.FindById(gameId);
        var cashier = _cashierRepository.FindById(cashierId);

        return new Ticket(id, game, customerName, customerAddress, cashier, noOfSeats, price);
    }
}
