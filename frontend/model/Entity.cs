namespace frontend.model;

public class Entity<TId>
{
    public TId Id { get; set; }

    public Entity(TId id)
    {
        Id = id;
    }
}