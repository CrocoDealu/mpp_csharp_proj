using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using frontend.model;
using frontend.utils;
using frontend.network;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace frontend.views;

public partial class TicketWindow : Window, Listener
{
        public TicketWindow()
        {
            InitializeComponent();
            
            Resources.Add("MatchConverter", new TeamVsTeamConverter());

            ticketsTable = this.Find<ListBox>("ticketsTable");
            nameField = this.Find<TextBox>("nameField");
            addressField = this.Find<TextBox>("addressField");
            LoadTickets();
            ConnectionManager.GetDispatcher().OnEvent("TICKETS", message =>
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    LoadTickets();
                });
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        private void LoadTickets()
        {
            // var tickets = _service.GetTicketsForClient(null);
            // ticketsTable.ItemsSource = tickets.ToList();
            string name;
            if (nameField == null || nameField.Text == null)
            {
                name = "";
            }
            else
            {
                name = nameField.Text;
            }
            string address;
            if (addressField == null || addressField.Text == null)
            {
                address = "";
            }
            else
            {
                address = addressField.Text;
            }
            JObject request = new JObject();
            request["type"] = "GET_TICKETS";
            request["messageId"] = ConnectionManager.GetMessageId();
            JObject payload = new JObject();
            payload["username"] = name;
            payload["address"] = address;
            request["payload"] = payload;
            FrontendClient frontendClient  = ConnectionManager.GetClient();
            try
            {
                string requestString = JsonConvert.SerializeObject(request, Formatting.None);
                frontendClient.send(requestString);
                ConnectionManager.GetDispatcher().AddPendingRequest(request).Task.ContinueWith(response =>
                {
                    object result = ConnectionManager.GetResponseParser().HandleResponse(response.Result.ToString());
                    if (result is IEnumerable<Ticket> tickets)
                    {
                        var ticketListItemsSource = tickets as Ticket[] ?? tickets.ToArray();
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            ticketsTable.ItemsSource = ticketListItemsSource;
                        });
                    }
                    else
                    {
                        ticketsTable.ItemsSource = null;
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        private void OnSearchPressed(object sender, RoutedEventArgs e)
        {
            if (ticketsTable == null)
            {
                ticketsTable = this.Find<ListBox>("ticketsTable");
            }
            LoadTickets();
        }

        public void onUpdate(string message)
        {
            LoadTickets();
        }
}