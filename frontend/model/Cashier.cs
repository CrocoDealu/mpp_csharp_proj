using Newtonsoft.Json;

namespace frontend.model;

public class Cashier: Entity<int>
{
    public String Name { get; set; }
    public String Username { get; set; }
    
    
    public Cashier(int id, string name, string username) : base(id)
    {
        Name = name;
        Username = username;
    }

    public Cashier() : base(0)
    {
        Name = "";
        Username = "";
    }

    public Cashier(int cashierId) : base(cashierId)
    {
        Name = "";
        Username = "";
    }

    public override string ToString()
    {
        return $"id = {Id}, name = {Name}, username = {Username}";
    }
}