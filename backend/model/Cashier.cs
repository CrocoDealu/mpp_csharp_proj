using Newtonsoft.Json;

namespace ConsoleApp1.model;

public class Cashier: Entity<int>
{
    public String Name { get; set; }
    public String Username { get; set; }
    
    
    [JsonIgnore]
    public String Password { get; set; }

    public Cashier(int id, string name, string username, string password) : base(id)
    {
        Name = name;
        Username = username;
        Password = password;
    }

    public Cashier(int cashierId) : base(cashierId)
    {
        Name = "";
        Username = "";
        Password = "";
    }

    public override string ToString()
    {
        return $"id = {Id}, name = {Name}, username = {Username}, password = {Password}";
    }
}