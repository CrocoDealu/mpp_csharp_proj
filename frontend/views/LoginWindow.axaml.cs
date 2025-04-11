using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ConsoleApp1.model;
using ConsoleApp1.service;

namespace ConsoleApp1.views;


public partial class LoginWindow : Window
{
    
    private SportsTicketManagementService _service;
    public LoginWindow(SportsTicketManagementService service)
    {
        InitializeComponent();
        _service = service;
    }

    private void OnLoginClick(object sender, RoutedEventArgs e)
    {
        string username = usernameInput.Text;
        string password = passwordInput.Text;
        Cashier? cashier = _service.GetCashierByUsername(username);
        

        if (cashier != null)
        {
            if (password == cashier.Password)
            {
                OpenMainPanel(cashier);
            }
            else
            {
                ClearFields();
                passwordError.Content = "Wrong password";
            }
        }
        else
        {
            ClearFields();
            usernameError.Content = "Username not found!";
        }
        
    }

    private void OpenMainPanel(Cashier cashier)
    {
        var mainPanel = new MainWindow(_service, cashier);
        mainPanel.Show();
        this.Close();
    }

    private void ClearFields()
    {
        usernameError.Content = "";
        passwordError.Content = "";
    }
}