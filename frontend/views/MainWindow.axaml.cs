using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using ConsoleApp1.model;
using ConsoleApp1.utils;
using frontend.network;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleApp1.views;

public partial class MainWindow : Window
{
    private Cashier _loggedCashier;
    private Game _selectedGame;
    private readonly List<Window> openedStages = new();
    private object lockObject = new object();
    
    public MainWindow(Cashier loggedCashier)
    {
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
        JObject request = new JObject();
        request["type"] = "GET_GAMES";
        request["messageId"] = ConnectionManager.GetMessageId();
        FrontendClient frontendClient  = ConnectionManager.GetClient();
        try
        {
            string requestString = JsonConvert.SerializeObject(request);
            frontendClient.send(requestString);
            ConnectionManager.GetDispatcher().AddPendingRequest(request).Task.ContinueWith(response =>
            {
                try
                {
                    object responseHandled = ConnectionManager.GetResponseParser().HandleResponse(response.Result.ToString());
                    if (responseHandled is IEnumerable<Game> games)
                    {
                        var matchListItemsSource = games as Game[] ?? games.ToArray();
                        foreach (var VARIABLE in matchListItemsSource)
                        {
                            Console.WriteLine(VARIABLE);
                        }
                        Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            matchList.ItemsSource = matchListItemsSource;
                        });
                    }
                    else
                    {
                        Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            matchList.ItemsSource = new List<Match>();
                        });
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
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
        EndSession();
        CloseOpenedScenes();
        var loginPanel = new LoginWindow();
        loginPanel.Show();
        this.Close();
    }

    private void EndSession() {
        FrontendClient frontendClient = ConnectionManager.GetClient();
        JObject jsonObject = new JObject();
        jsonObject["type"] = "LOGOUT";
        string jsonNotification = JsonConvert.SerializeObject(jsonObject, Formatting.None);
        frontendClient.send(jsonNotification);
        try {
            frontendClient.Close();
        } catch (IOException e) {
            Console.WriteLine(e);
        }
    }
    
    private void OnViewTickets(object? sender, RoutedEventArgs e)
    {
        var ticketPanel = new TicketWindow();
        lock (openedStages)
        {
            openedStages.Add(ticketPanel);
        }
        ticketPanel.Show();
    }

    private void CloseOpenedScenes()
    {
        lock (openedStages)
        {
            foreach (var window in openedStages)
            {
                window.Close();
            }
        }
    }
    
    private void OnSell(object? sender, RoutedEventArgs e)
    {
        bool canSell = canSellTicket(_selectedGame);
        if (canSell) {
            Ticket ticket = new Ticket(0, _selectedGame, clientName.Text, clientAddress.Text, _loggedCashier,(int) noOfSeats.Value);
            // _service.SaveTicket(ticket);
            _selectedGame.Capacity -= (int) noOfSeats.Value;
            // _service.UpdateGame(_selectedGame);
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