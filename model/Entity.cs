namespace ConsoleApp1.model;

public class Entity<TId>
{
    public TId Id { get; }

    public Entity(TId id)
    {
        Id = id;
    }
}