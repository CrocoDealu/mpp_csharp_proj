using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using frontend.model;
using frontend.network;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Avalonia.Threading;

namespace frontend.views;


public partial class LoginWindow : Window
{
    
    public LoginWindow()
    {
        InitializeComponent();
    }

    private void OnLoginClick(object sender, RoutedEventArgs e)
    {
        string username = usernameInput.Text;
        string password = passwordInput.Text;
        JObject request = new JObject();
        JObject requestPayload = new JObject();
        requestPayload["username"] = username;
        requestPayload["password"] = password;
        request["payload"] = requestPayload;
        request["type"] = "LOGIN";
        request["messageId"] = ConnectionManager.GetMessageId();
        FrontendClient frontendClient = ConnectionManager.GetClient();
        try
        {
            string requestString = JsonConvert.SerializeObject(request);
            ConnectionManager.GetDispatcher().AddPendingRequest(request).Task.ContinueWith(task =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    JObject response = task.Result;
                    object responseHandled = ConnectionManager.GetResponseParser().HandleResponse(response.ToString());
                    
                    if (responseHandled is Tuple<Cashier, string> responseTuple)
                    {
                        string reason = responseTuple.Item2;
                        Cashier cashier = responseTuple.Item1;

                        if (reason == "USER_NOT_FOUND")
                        {
                            usernameError.Content = "Username not found";
                        }
                        else if (reason == "INCORRECT_PASSWORD")
                        {
                            passwordError.Content = "Wrong password!";
                        }
                        else
                        {
                            OpenMainPanel(cashier);
                        }
                    }
                }
                else if (task.Status == TaskStatus.Faulted)
                {
                    Console.WriteLine($"Task failed with exception: {task.Exception}");
                }
                else if (task.Status == TaskStatus.Canceled)
                {
                    Console.WriteLine("Task was canceled.");
                }
            });
            frontendClient.send(requestString);
            
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        
    }

    private void OpenMainPanel(Cashier cashier)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var mainPanel = new MainWindow(cashier);
            mainPanel.Show();
            Close();
        });
    }

    private void ClearFields()
    {
        usernameError.Content = "";
        passwordError.Content = "";
    }
}