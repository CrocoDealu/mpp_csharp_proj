using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ConsoleApp1.model;
using ConsoleApp1.service;
using ConsoleApp1.utils;

namespace ConsoleApp1.views;

public partial class MainWindow : Window
{
    private SportsTicketManagementService _service;
    private Cashier _loggedCashier;
    private Game _selectedGame;

    public MainWindow(SportsTicketManagementService service, Cashier loggedCashier)
    {
        _service = service;
        _loggedCashier = loggedCashier;
        
        InitializeComponent();

        Resources.Add("TeamVsTeamConverter", new TeamVsTeamConverter());
        Resources.Add("CapacityConverter", new CapacityConverter());
        Resources.Add("CapacityColorConverter", new CapacityColorConverter());
    
        noOfSeats.Increment = 1;
        noOfSeats.Minimum = 1;
        noOfSeats.FormatString = "F0";
        noOfSeats.AllowSpin = true;
        noOfSeats.ShowButtonSpinner = true;
        sellButton.IsEnabled = false;
        InitializeMatches();
    }

    private void InitializeMatches()
    {
        var games = _service.GetAllGames();
        matchList.ItemsSource = games;

        matchList.SelectionChanged += MatchList_SelectionChanged;
    }

    private void MatchList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (matchList.SelectedItem is Game game)
        {
            _selectedGame = game;
            forGameLabel.Text = $"{game.Team1} vs {game.Team2}";
            UpdateSpinnerMaxValue(game.Capacity);
            sellButton.IsEnabled = game.Capacity > 0;
        }
    }

    private void UpdateSpinnerMaxValue(int capacity)
    {
        noOfSeats.Maximum = capacity;
        if (noOfSeats.Value > capacity)
        {
            noOfSeats.Value = capacity;
        }
    }

    private void OnLogOut(object? sender, RoutedEventArgs e)
    {
        var loginPanel = new LoginWindow(_service);
        loginPanel.Show();
        this.Close();
    }

    private void OnViewTickets(object? sender, RoutedEventArgs e)
    {
        var ticketPanel = new TicketWindow(_service);
        ticketPanel.Show();
    }

    private void OnSell(object? sender, RoutedEventArgs e)
    {
        bool canSell = canSellTicket(_selectedGame);
        if (canSell) {
            Ticket ticket = new Ticket(0, _selectedGame, clientName.Text, clientAddress.Text, _loggedCashier,(int) noOfSeats.Value);
            _service.SaveTicket(ticket);
            _selectedGame.Capacity -= (int) noOfSeats.Value;
            _service.UpdateGame(_selectedGame);
            InitializeMatches();
            clearClientInfo();
        }
    }

    private void clearClientInfo()
    {
        clientName.Text = "";
        clientAddress.Text = "";
        noOfSeats.Value = 1;
    }

    private bool canSellTicket(Game selectedGame)
    {
        if (selectedGame != null) {
            bool clientNameEmpty = string.IsNullOrEmpty(clientName.Text);
            bool clientAddressEmpty = string.IsNullOrEmpty(clientAddress.Text);
            bool noOfSeatsEmpty = noOfSeats.Value == 0;
            if (!clientNameEmpty && !clientAddressEmpty && !noOfSeatsEmpty) {
                clientName.Classes.Remove("error");
                clientAddress.Classes.Remove("error");
                noOfSeats.Classes.Remove("error");
                return selectedGame.Capacity != 0;
            } else
            {
                if (clientNameEmpty)
                    clientName.Classes.Add("error");
                else
                    clientName.Classes.Remove("error");

                if (clientAddressEmpty)
                    clientAddress.Classes.Add("error");
                else
                    clientAddress.Classes.Remove("error");

                if (noOfSeatsEmpty)
                    noOfSeats.Classes.Add("error");
                else
                    noOfSeats.Classes.Remove("error");
            }
        }
        return false;
    }
}