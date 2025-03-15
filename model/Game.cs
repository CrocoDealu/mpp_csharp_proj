namespace ConsoleApp1.model;

public class Game: Entity<int>
{
    public string Team1 { get; set; }
    public string Team2 { get; set; }
    public string Team1Score { get; set; }
    public string Team2Score { get; set; }
    public string Competition { get; set; }
    public int Capacity { get; set; }
    public string Stage { get; set; }

    public Game(int id, string team1, string team2, string team1Score, string team2Score, string competition, int capacity, string stage) : base(id)
    {
        Team1 = team1;
        Team2 = team2;
        Team1Score = team1Score;
        Team2Score = team2Score;
        Competition = competition;
        Capacity = capacity;
        Stage = stage;
    }
}