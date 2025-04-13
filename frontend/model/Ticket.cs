namespace frontend.model;

public class Ticket: Entity<int>
{
    public Game Game { get; set; }
    public string CustomerName { get; set; }
    public string CustomerAddress { get; set; }
    public Cashier Seller { get; set; }
    public int NoOfSeats { get; set; }
    public Ticket(int id, Game game, string customerName, string customerAddress, Cashier seller, int noOfSeats) : base(id)
    {
        Game = game;
        CustomerName = customerName;
        CustomerAddress = customerAddress;
        Seller = seller;
        NoOfSeats = noOfSeats;
    }

    public override string ToString()
    {
        return $"id = {Id}, game = {Game.Id}, customer name = {CustomerName}, customer address = {CustomerAddress}, seller = {Seller.Id}, no of seats = {NoOfSeats}";
    }
}