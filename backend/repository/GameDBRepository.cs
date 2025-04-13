using System.Data;
using System.Reflection;
using backend.model;
using backend.utils;
using log4net;
using Microsoft.Data.Sqlite;

namespace backend.repository;

public class GameDBRepository : IGameRepository
{
    private readonly DbUtils _dbUtils;
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

    public GameDBRepository(DbUtils dbUtils)
    {
        _dbUtils = dbUtils;
    }

    public IEnumerable<Game> FindAll()
    {
        _logger.Info("Attempting to retrieve all games");
        var connection = _dbUtils.GetConnection();
        var games = new List<Game>();
        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Games";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Game game = MapReaderToEntity(reader);
                    games.Add(game);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"An exception occurred while retrieving all games: {ex.Message}");
            throw new InvalidOperationException("An exception occurred while retrieving all games", ex);
        }
        _logger.Info("Successfully retrieved all games");
        return games;
    }

    public Game? FindById(int id)
    {
        _logger.Info($"Attempting to find game with ID {id}");
        var connection = _dbUtils.GetConnection();
        Game? game = null;
        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Games WHERE Id = @id";
                command.Parameters.Add(new SqliteParameter("@id", id));
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    game = MapReaderToEntity(reader);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"An exception occurred while retrieving game with id {id}: {ex.Message}");
            throw new InvalidOperationException("An exception occurred while retrieving the game", ex);
        }
        return game;
    }

    public Game Save(Game entity)
    {
        _logger.Info("Attempting to save game");
        var connection = _dbUtils.GetConnection();
        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Games (team1, team2, team_1_score, team_2_score, competition, capacity, stage, price) VALUES (@team1, @team2, @team1Score, @team2Score, @competition, @capacity, @stage, @price); SELECT last_insert_rowid();";

                command.Parameters.Add(new SqliteParameter("@team1", entity.Team1));
                command.Parameters.Add(new SqliteParameter("@team2", entity.Team2));
                command.Parameters.Add(new SqliteParameter("@team1Score", entity.Team1Score));
                command.Parameters.Add(new SqliteParameter("@team2Score", entity.Team2Score));
                command.Parameters.Add(new SqliteParameter("@competition", entity.Competition));
                command.Parameters.Add(new SqliteParameter("@capacity", entity.Capacity));
                command.Parameters.Add(new SqliteParameter("@stage", entity.Stage));
                command.Parameters.Add(new SqliteParameter("@price", entity.TicketPrice));

                var generatedId = command.ExecuteScalar();
                int id = Convert.ToInt32(generatedId);
                entity.Id = id;
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"An exception occurred while saving game: {ex.Message}");
            throw new InvalidOperationException("An exception occurred while saving the game", ex);
        }
        return entity;
    }

    public Game? DeleteById(int id)
    {
        Game? game = FindById(id);
        if (game == null)
        {
            _logger.Info($"No game found with ID {id}");
            throw new InvalidOperationException($"No game found with ID {id}");
        }
        _logger.Info($"Attempting to delete game with ID {id}");
        var connection = _dbUtils.GetConnection();
        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM Games WHERE Id = @id";
                command.Parameters.Add(new SqliteParameter("@id", id));
                command.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"An exception occurred while deleting game with id {id}: {ex.Message}");
            throw new InvalidOperationException("An exception occurred while deleting the game", ex);
        }
        return game;
    }

    public Game Update(Game entity)
    {
        _logger.Info($"Attempting to update game with ID {entity.Id}");
        var connection = _dbUtils.GetConnection();
        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Games SET team1 = @team1, team2 = @team2, team_1_score = @team1Score, team_2_score = @team2Score, competition = @competition, capacity = @capacity, stage = @stage, price = @price WHERE Id = @id";

                command.Parameters.Add(new SqliteParameter("@team1", entity.Team1));
                command.Parameters.Add(new SqliteParameter("@team2", entity.Team2));
                command.Parameters.Add(new SqliteParameter("@team1Score", entity.Team1Score));
                command.Parameters.Add(new SqliteParameter("@team2Score", entity.Team2Score));
                command.Parameters.Add(new SqliteParameter("@competition", entity.Competition));
                command.Parameters.Add(new SqliteParameter("@capacity", entity.Capacity));
                command.Parameters.Add(new SqliteParameter("@stage", entity.Stage));
                command.Parameters.Add(new SqliteParameter("@price", entity.TicketPrice));
                command.Parameters.Add(new SqliteParameter("@id", entity.Id));

                command.ExecuteNonQuery();
            }
            return entity;
        }
        catch (Exception ex)
        {
            _logger.Error($"An exception occurred while updating game with id {entity.Id}: {ex.Message}");
            throw new InvalidOperationException("An exception occurred while updating the game", ex);
        }
    }

    public IEnumerable<Game> findGamesForTickets(IEnumerable<Ticket> tickets)
    {
        throw new NotImplementedException();
    }

    private Game MapReaderToEntity(IDataReader reader)
    {
        int id = reader.GetInt32(0);
        string team1 = reader.GetString(1);
        string team2 = reader.GetString(2);
        int team1Score = reader.GetInt32(3);
        int team2Score = reader.GetInt32(4);
        string competition = reader.GetString(5);
        int capacity = reader.GetInt32(6);
        string stage = reader.GetString(7);
        float price = reader.GetFloat(8);
        return new Game(id, team1, team2, team1Score, team2Score, competition, capacity, stage, price);
    }
}