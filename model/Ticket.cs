namespace ConsoleApp1.model;

public class Ticket: Entity<int>
{
    public Game Game { get; set; }
    public string CustomerName { get; set; }
    public string CustomerAddress { get; set; }
    public Cashier Seller { get; set; }
    public int NoOfSeats { get; set; }
    public float Price { get; set; }

    public Ticket(int id, Game game, string customerName, string customerAddress, Cashier seller, int noOfSeats, float price) : base(id)
    {
        Game = game;
        CustomerName = customerName;
        CustomerAddress = customerAddress;
        Seller = seller;
        NoOfSeats = noOfSeats;
        Price = price;
    }
}