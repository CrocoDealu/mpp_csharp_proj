namespace backend.service;

public interface IService<T>
{
    public void NotifyClients(string message);
    public void LoginClient(T client);
    public void LogoutClient(T client);
}