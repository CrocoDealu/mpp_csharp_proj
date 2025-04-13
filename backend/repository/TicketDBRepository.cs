using System.Data;
using System.Reflection;
using System.Text;
using backend.dto;
using backend.model;
using backend.utils;
using log4net;
using Microsoft.Data.Sqlite;

namespace backend.repository;

public class TicketDBRepository : ITicketRepository
{
    private readonly DbUtils _dbUtils;
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
    private readonly IGameRepository _gameRepository;
    private readonly ICashierRepository _cashierRepository;

    public TicketDBRepository(DbUtils dbUtils, IGameRepository gameRepository, ICashierRepository cashierRepository)
    {
        _dbUtils = dbUtils;
        _gameRepository = gameRepository;
        _cashierRepository = cashierRepository;
    }

    public Ticket? FindById(int id)
    {
        _logger.Info($"Attempting to find ticket with ID {id}");
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
            _logger.Error($"An exception occurred while retrieving ticket with id {id}: {ex.Message}");
            throw new InvalidOperationException("An exception occurred while retrieving the ticket", ex);
        }
        return ticket;
    }

    public IEnumerable<Ticket> FindAll()
    {
        _logger.Info("Attempting to retrieve all tickets");
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
            _logger.Error($"An exception occurred while retrieving all tickets: {ex.Message}");
            throw new InvalidOperationException("An exception occurred while retrieving all tickets", ex);
        }
        _logger.Info("Successfully retrieved all tickets");
        return tickets;
    }

    public Ticket Save(Ticket entity)
    {
        _logger.Info("Attempting to save ticket");
        var connection = _dbUtils.GetConnection();
        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Tickets (game_id, customer_name, customer_address, cashier_id, no_of_seats) VALUES (@gameId, @customerName, @customerAddress, @cashierId, @noOfSeats); SELECT last_insert_rowid();";

                command.Parameters.Add(new SqliteParameter("@gameId", entity.Game.Id));
                command.Parameters.Add(new SqliteParameter("@customerName", entity.CustomerName));
                command.Parameters.Add(new SqliteParameter("@customerAddress", entity.CustomerAddress));
                command.Parameters.Add(new SqliteParameter("@cashierId", entity.Seller.Id));
                command.Parameters.Add(new SqliteParameter("@noOfSeats", entity.NoOfSeats));

                var generatedId = command.ExecuteScalar();
                int id = Convert.ToInt32(generatedId);
                entity.Id = id;
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"An exception occurred while saving ticket: {ex.Message}");
            throw new InvalidOperationException("An exception occurred while saving the ticket", ex);
        }
        return entity;
    }

    public Ticket? DeleteById(int id)
    {
        Ticket? ticket = FindById(id);
        if (ticket == null)
        {
            _logger.Info($"No ticket found with ID {id}");
            throw new InvalidOperationException($"No ticket found with ID {id}");
        }
        _logger.Info($"Attempting to delete ticket with ID {id}");
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
            _logger.Error($"An exception occurred while deleting ticket with id {id}: {ex.Message}");
            throw new InvalidOperationException("An exception occurred while deleting the ticket", ex);
        }
        return ticket;
    }

    public Ticket Update(Ticket entity)
    {
        _logger.Info($"Attempting to update ticket with ID {entity.Id}");
        var connection = _dbUtils.GetConnection();
        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Tickets SET game_id = @gameId, customer_name = @customerName, customer_address = @customerAddress, cashier_id = @cashierId, no_of_seats = @noOfSeats WHERE Id = @id";

                command.Parameters.Add(new SqliteParameter("@gameId", entity.Game.Id));
                command.Parameters.Add(new SqliteParameter("@customerName", entity.CustomerName));
                command.Parameters.Add(new SqliteParameter("@customerAddress", entity.CustomerAddress));
                command.Parameters.Add(new SqliteParameter("@cashierId", entity.Seller.Id));
                command.Parameters.Add(new SqliteParameter("@noOfSeats", entity.NoOfSeats));
                command.Parameters.Add(new SqliteParameter("@id", entity.Id));

                command.ExecuteNonQuery();
            }
            return entity;
        }
        catch (Exception ex)
        {
            _logger.Error($"An exception occurred while updating ticket with id {entity.Id}: {ex.Message}");
            throw new InvalidOperationException("An exception occurred while updating the ticket", ex);
        }
    }

    public IEnumerable<Ticket> GetTicketsSoldForGame(Game game)
    {
        _logger.Info($"Attempting to retrieve tickets sold for game with ID {game.Id}");
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
            _logger.Error($"An exception occurred while retrieving tickets for game with id {game.Id}: {ex.Message}");
            throw new InvalidOperationException("An exception occurred while retrieving tickets for the game", ex);
        }
        _logger.Info($"Successfully retrieved tickets for game with ID {game.Id}");
        return tickets;
    }

    public IEnumerable<Ticket> GetTicketsForClient(ClientFilterDTO filter)
    {
        _logger.Info($"Attempting to get all tickets for client");
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
            _logger.Info($"An exception occured when getting all tickets for client {ex.Message}");
            throw new InvalidOperationException("An exception occurred when getting all tickets for client", ex);
        }
        return tickets;
    }

    private string ProcessFilter(string sql, ClientFilterDTO filter)
    {
        StringBuilder modified = new StringBuilder(sql);
        if (filter != null && (filter.ClientName != null || filter.ClientAddress != null))
        {
            modified.Append(" WHERE ");
            List<string> parameters = new List<string>();
            List<string> values = new List<string>();
    
            if (filter.ClientAddress != null && filter.ClientAddress.Length > 0)
            {
                parameters.Add("customer_address = ");
                values.Add(filter.ClientAddress);
            }
    
            if (filter.ClientName != null && filter.ClientName.Length > 0)
            {
                parameters.Add("customer_name = ");
                values.Add(filter.ClientName);
            }
    
            for (int i = 0; i < parameters.Count; i++)
            {
                if (i != parameters.Count - 1)
                {
                    modified.Append(parameters[i]).Append("'").Append(values[i]).Append("'").Append(" AND ");
                }
                else
                {
                    modified.Append(parameters[i]).Append("'").Append(values[i]).Append("'");
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
        var game = _gameRepository.FindById(gameId);
        var cashier = _cashierRepository.FindById(cashierId);

        return new Ticket(id, game, customerName, customerAddress, cashier, noOfSeats);
    }
}
