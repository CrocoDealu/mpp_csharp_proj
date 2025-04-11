using backend.network;
using ConsoleApp1.dto;
using ConsoleApp1.model;
using ConsoleApp1.repository;
using Newtonsoft.Json.Linq;

namespace ConsoleApp1.service;

public class SportsTicketManagementService
{
    private ICashierRepository cashierRepository;
    private IGameRepository gameRepository;
    private ITicketRepository ticketRepository;
    private ClientManager clientManager;

    public SportsTicketManagementService(ICashierRepository cashierRepository, IGameRepository gameRepository, ITicketRepository ticketRepository, ClientManager clientManager)
    {
        this.cashierRepository = cashierRepository;
        this.gameRepository = gameRepository;
        this.ticketRepository = ticketRepository;
        this.clientManager = clientManager;
    }

    public IEnumerable<Ticket> GetTicketsForClient(ClientFilterDTO filter)
    {
        return ticketRepository.GetTicketsForClient(filter);
    }

    public Ticket? SaveTicket(Ticket ticket)
    {
        var gameOptional = GetGameById(ticket.Game.Id);

        if (gameOptional != null)
        {
            var game = gameOptional;
            game.Capacity -= ticket.NoOfSeats;
            UpdateGame(game);
            var savedTicket = ticketRepository.Save(ticket);

            var updateTicketListMessage = new JObject
            {
                ["type"] = "TICKETS"
            };
            String updateNotifyT = updateTicketListMessage.ToString(Newtonsoft.Json.Formatting.None);
            NotifyClients(updateNotifyT);

            var updateGameListMessage = new JObject
            {
                ["type"] = "GAMES"
            };
            String updateNotifyG = updateGameListMessage.ToString(Newtonsoft.Json.Formatting.None);
            NotifyClients(updateNotifyG);

            return savedTicket;
        }
        return null;
    }

    public Cashier? GetCashierByUsername(string username)
    {
        return cashierRepository.FindByUsername(username);
    }

    public Game? GetGameById(int gameId)
    {
        return gameRepository.FindById(gameId);
    }
    
    public Game? UpdateGame(Game game)
    {
        return gameRepository.Update(game);
    }

    public IEnumerable<Game> GetAllGames()
    {
        return gameRepository.FindAll();
    }
    
    public void LoginClient(BackendClient client) {
        clientManager.Subscribe(client);
    }

    public void LogoutClient(BackendClient client) {
        try {
            clientManager.Unsubscribe(client);
            client.Close();
        } catch (Exception e) {
            Console.WriteLine(e);
        }
    }

    public void NotifyClients(String notification) {
        clientManager.NotifySubscribers(notification);
    }
    
}