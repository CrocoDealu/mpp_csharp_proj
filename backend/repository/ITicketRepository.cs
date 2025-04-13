using backend.dto;
using backend.model;

namespace backend.repository;

public interface ITicketRepository: IRepository<int, Ticket>
{
    public IEnumerable<Ticket> GetTicketsSoldForGame(Game game);
    public IEnumerable<Ticket> GetTicketsForClient(ClientFilterDTO filter);
}