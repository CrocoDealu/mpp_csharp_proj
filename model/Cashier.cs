namespace ConsoleApp1.model;

public class Cashier: Entity<int>
{
    public String Name { get; set; }
    public String Username { get; set; }
    public String Password { get; set; }

    public Cashier(int id, string name, string username, string password) : base(id)
    {
        Name = name;
        Username = username;
        Password = password;
    }
}