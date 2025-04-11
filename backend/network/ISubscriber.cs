namespace backend.network;

public interface ISubscriber
{
    public void OnNotify(String notification);
}