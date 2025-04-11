using System.Net.Sockets;

namespace backend.network;

public class BackendClient : ISubscriber, IDisposable
{
    private TcpClient _client;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;
    private bool _disposed = false;
    public BackendClient(TcpClient tcpClient)
    {
        _client = tcpClient;

        NetworkStream networkStream = _client.GetStream();
        _reader = new StreamReader(networkStream);
        _writer = new StreamWriter(networkStream) { AutoFlush = true };
    }

    public void Send(String message)
    {
        _writer.WriteLine(message);
    }
    
    public string Receive()
    {
        try
        {
            if (!IsConnected() || !_client.Connected)
            {
                return null;
            }

            return _reader.ReadLine();
        }
        catch (IOException)
        {
            Close();
            return null;
        }
    }

    public void Close()
    {
        try
        {
            _reader?.Close();
            _writer?.Close();
            _client?.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error closing resources: {e.Message}");
        }
    }
    
    
    public bool IsConnected()
    {
        return _client.Connected && !_client.Client.Poll(1, SelectMode.SelectRead) && _client.Client.Available == 0;
    }

    public bool IsClosed()
    {
        return !_client.Connected;
    }
    
    public void OnNotify(string notification)
    {
        Send(notification);
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    ~BackendClient()
    {
        Dispose(false);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _client?.Dispose();
            }

            _disposed = true;
        }
    }
}