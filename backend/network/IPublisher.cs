namespace backend.network;
public interface IPublisher<T>
{
    void Subscribe(T client);
    void Unsubscribe(T client);
    void NotifySubscribers(string changeNotification);
}