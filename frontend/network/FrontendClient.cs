using System.Net.Sockets;
using Avalonia.Threading;
using frontend.utils;
using Newtonsoft.Json.Linq;

namespace frontend.network;

public class FrontendClient
{
    private TcpClient _client;
    private StreamReader _reader;
    private StreamWriter _writer;
    private List<Listener> _listeners = new List<Listener>();
    private Thread _listenerThread;
    public FrontendClient(string host, int port)
    {
        _client = new TcpClient(host, port);
        _reader = new StreamReader(_client.GetStream());
        _writer = new StreamWriter(_client.GetStream())
        {
            AutoFlush = true
        };
        StartListenerThread();
    }

    private void StartListenerThread()
    {
        _listenerThread = new Thread(() =>
        {
            while (!IsClosed())
            {
                
                try
                {
                    String jsonString = _reader.ReadLine();
                    if (jsonString == null)
                    {
                        Console.WriteLine("Connection closed");
                        break;
                    }
                    
                    if (!jsonString.Equals(""))
                    {
                        JObject message = JObject.Parse(jsonString);
                        Dispatcher.UIThread.Post(() =>
                        {
                            ConnectionManager.GetDispatcher().Dispatch(message);
                        });
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        });
        _listenerThread.IsBackground = true;
        _listenerThread.Start();
    }

    public void send(string message)
    {
        _writer.WriteLine(message);
    }

    public async Task Close()
    {
        if (_listenerThread != null)
        {
            try
            {
                _listenerThread.Join(1000);
            }
            catch (Exception e)
            {
                Console.WriteLine("Interrupted while waiting for the listener thread to finish");
            }
        }

        try
        {
            if (_reader != null)
            {
                _reader.Close();
            }

            if (_writer != null)
            {
                _writer.Close();
            }

            if (_client != null)
            {
                _client.Close();
            }
            _listeners.Clear();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    public bool IsClosed() {
        return !_client.Connected;
    }
    
}