using backend.model;

namespace backend.repository;

public interface IGameRepository: IRepository<int, Game>
{
    public IEnumerable<Game> findGamesForTickets(IEnumerable<Ticket> tickets);
}