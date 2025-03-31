using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ConsoleApp1.dto;
using ConsoleApp1.model;
using ConsoleApp1.service;
using ConsoleApp1.utils;

namespace ConsoleApp1.views;

public partial class TicketWindow : Window
{
    private SportsTicketManagementService _service;
        public TicketWindow(SportsTicketManagementService service)
        {
            this._service = service;
            InitializeComponent();
            
            Resources.Add("MatchConverter", new TeamVsTeamConverter());

            ticketsTable = this.Find<ListBox>("ticketsTable");
            nameField = this.Find<TextBox>("nameField");
            addressField = this.Find<TextBox>("addressField");
            LoadTickets();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        private void LoadTickets()
        {
            var tickets = _service.GetTicketsForClient(null);
            ticketsTable.ItemsSource = tickets.ToList();
        }
        
        private List<Ticket> GetTickets()
        {
            return new List<Ticket>();
        }
        
        private void OnSearchPressed(object sender, RoutedEventArgs e)
        {
            try
            {
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
                
                
                var filteredTickets = SearchTickets(name, address);
                if (ticketsTable == null)
                {
                    ticketsTable = this.Find<ListBox>("ticketsTable");
                }
                ticketsTable.ItemsSource = filteredTickets;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
        private List<Ticket> SearchTickets(string name, string address)
        {
            ClientFilterDTO filter;
            if (name.Length == 0 && address.Length == 0)
            {
                filter = null;
            }
            else
            {
                filter = new ClientFilterDTO(name, address);
            }
            return _service.GetTicketsForClient(filter).ToList();
        }
    }