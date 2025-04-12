namespace ConsoleApp1.model;

public class Game: Entity<int>
{
    public string Team1 { get; set; }
    public string Team2 { get; set; }
    public int Team1Score { get; set; }
    public int Team2Score { get; set; }
    public string Competition { get; set; }
    public int Capacity { get; set; }
    public string Stage { get; set; }
    
    public float TicketPrice { get; set; }

    public Game(int id, string team1, string team2, int team1Score, int team2Score, string competition, int capacity, string stage, float ticketPrice) : base(id)
    {
        Team1 = team1;
        Team2 = team2;
        Team1Score = team1Score;
        Team2Score = team2Score;
        Competition = competition;
        Capacity = capacity;
        Stage = stage;
        TicketPrice = ticketPrice;
    }

    public override string ToString()
    {
        return $"id = {Id}, team1 = {Team1}, team2 = {Team2}, team1 Score = {Team1Score}, team2 Score = {Team2Score}, competition = {Competition}, capacity = {Capacity}, stage = {Stage}, Price = {TicketPrice}";
    }
}