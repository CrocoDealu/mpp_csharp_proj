namespace backend.network;

public class ClientManager : IPublisher<BackendClient>
{
    private static readonly HashSet<BackendClient> ConnectedClients = new HashSet<BackendClient>();

    private static readonly object LockObject = new object();

    public void Subscribe(BackendClient client)
    {
        lock (LockObject)
        {
            ConnectedClients.Add(client);
        }
    }

    public void Unsubscribe(BackendClient client)
    {
        lock (LockObject)
        {
            ConnectedClients.Remove(client);
        }
    }

    public void NotifySubscribers(string changeNotification)
    {
        lock (LockObject)
        {
            foreach (var client in ConnectedClients)
            {
                try
                {
                    client.OnNotify(changeNotification);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to notify client: {e.Message}");
                }
            }
        }
    }
}