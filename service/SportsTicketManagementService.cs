using Avalonia.Data;
using ConsoleApp1.dto;
using ConsoleApp1.model;
using ConsoleApp1.repository;

namespace ConsoleApp1.service;

public class SportsTicketManagementService
{
    private ICashierRepository cashierRepository;
    private IGameRepository gameRepository;
    private ITicketRepository ticketRepository;

    public SportsTicketManagementService(ICashierRepository cashierRepository, IGameRepository gameRepository, ITicketRepository ticketRepository)
    {
        this.cashierRepository = cashierRepository;
        this.gameRepository = gameRepository;
        this.ticketRepository = ticketRepository;
    }

    public IEnumerable<Ticket> GetTicketsForClient(ClientFilterDTO filter)
    {
        return ticketRepository.GetTicketsForClient(filter);
    }

    public Ticket? SaveTicket(Ticket ticket)
    {
        return ticketRepository.Save(ticket);
    }

    public Cashier? GetCashierByUsername(string username)
    {
        return cashierRepository.FindByUsername(username);
    }

    public Game? UpdateGame(Game game)
    {
        return gameRepository.Update(game);
    }

    public IEnumerable<Game> GetAllGames()
    {
        return gameRepository.FindAll();
    }
}