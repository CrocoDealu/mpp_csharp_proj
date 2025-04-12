using Newtonsoft.Json.Linq;

namespace frontend.network;

public class ConnectionManager
{
    private static FrontendClient _client;
    private static ResponseParser _responseParser = new();
    private static JSONDispatcher _dispatcher = new();
    private static int _idGenerator = 0;

    public static FrontendClient GetClient()
    {
        if (_client == null || _client.IsClosed())
        {
            _client = new FrontendClient("localhost", 1234);
        }
        return _client;
    }
    public static ResponseParser GetResponseParser()
    {
        return _responseParser;
    }

    public static JSONDispatcher GetDispatcher()
    {
        return _dispatcher;
    }
    public static int GetMessageId()
    {
        return Interlocked.Increment(ref _idGenerator);
    }
}