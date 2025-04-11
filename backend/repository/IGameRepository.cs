using ConsoleApp1.model;

namespace ConsoleApp1.repository;

public interface IGameRepository: IRepository<int, Game>
{
    public IEnumerable<Game> findGamesForTickets(IEnumerable<Ticket> tickets);
}