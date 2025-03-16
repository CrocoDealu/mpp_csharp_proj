using ConsoleApp1.dto;
using ConsoleApp1.model;

namespace ConsoleApp1.repository;

public interface ITicketRepository
{
    public IEnumerable<Ticket> GetTicketsSoldForGame(Game game);
    public IEnumerable<Ticket> GetTicketsForClient(ClientFilterDTO filter);
}