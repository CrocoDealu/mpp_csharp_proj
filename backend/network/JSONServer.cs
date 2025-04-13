using backend.service;

namespace backend.network;

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

public class JSONServer
{
    private readonly SportsTicketManagementService _sportsTicketManagementService;
    private readonly RequestHandler _requestHandler;

    public JSONServer(SportsTicketManagementService sportsTicketManagementService, RequestHandler requestHandler)
    {
        _sportsTicketManagementService = sportsTicketManagementService ?? throw new ArgumentNullException(nameof(sportsTicketManagementService));
        _requestHandler = requestHandler ?? throw new ArgumentNullException(nameof(requestHandler));
    }

    public void Run()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;

        var threadPool = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.None);

        TcpListener? server = null;
        try
        {
            server = new TcpListener(IPAddress.Any, 1234);
            server.Start();
            Console.WriteLine("Server is listening on port 1234");

            while (!token.IsCancellationRequested)
            {
                var tcpClient = server.AcceptTcpClient();
                Console.WriteLine("New client connected");

                threadPool.StartNew(() => HandleClient(tcpClient), token);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Server exception: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
        finally
        {
            server?.Stop();
            cancellationTokenSource.Cancel();
        }
    }

    private void HandleClient(TcpClient clientTcp)
    {
        using (var client = new BackendClient(clientTcp))
        {
            try
            {
                _sportsTicketManagementService.LoginClient(client);

                while (!client.IsClosed())
                {
                    try
                    {
                        var request = client.Receive();
                        if (request == null)
                        {
                            Console.WriteLine("Client disconnected");
                            _sportsTicketManagementService.LogoutClient(client);
                            break;
                        }

                        _requestHandler.HandleRequest(request, client);
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"Client connection lost: {ex.Message}");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
        }
    }
}