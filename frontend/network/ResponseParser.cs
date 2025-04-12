using ConsoleApp1.model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace frontend.network;

public class ResponseParser
{
    public object HandleResponse(string response)
    {
        JObject jsonObject = JObject.Parse(response);
        string type = jsonObject["type"]?.ToString();

        switch (type)
        {
            case "LOGIN_RESPONSE":
                return HandleLogin(jsonObject);
            case "GET_GAMES_RESPONSE":
                return HandleGetGames(jsonObject);
            case "GET_TICKETS_RESPONSE":
                return HandleGetTickets(jsonObject);
            case "ERROR":
                return "Error: " + jsonObject["message"]?.ToString();
            default:
                Console.WriteLine("Unknown response type: " + type);
                return null;
        }
    }

    private IEnumerable<Ticket> HandleGetTickets(JObject response)
    {
        JObject jsonObject = (JObject)response["payload"];
        string result = jsonObject["result"]?.ToString();

        try
        {
            return JsonConvert.DeserializeObject<List<Ticket>>(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return null;
    }

    private IEnumerable<Game> HandleGetGames(JObject response)
    {
        JObject jsonObject = (JObject)response["payload"];
        string result = jsonObject["result"]?.ToString();

        try
        {
            return JsonConvert.DeserializeObject<List<Game>>(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new List<Game>();
        }
    }

    private Tuple<Cashier, string> HandleLogin(JObject jsonObject)
    {
        JObject jsonPayload = (JObject)jsonObject["payload"];

        if (jsonPayload.ContainsKey("reason"))
        {
            string reason = jsonPayload["reason"]?.ToString();
            if (reason == "USER_NOT_FOUND")
            {
                return new Tuple<Cashier, string>(new Cashier(), "USER_NOT_FOUND");
            }
            else if (reason == "INCORRECT_PASSWORD")
            {
                return new Tuple<Cashier, string>(new Cashier(), "INCORRECT_PASSWORD");
            }
        }

        int id = jsonPayload["id"]?.ToObject<int>() ?? 0;
        string username = jsonPayload["username"]?.ToString();
        string name = jsonPayload["name"]?.ToString();
        Cashier cashier = new Cashier(id, username, name);
        return new Tuple<Cashier, string>(cashier, "");
    }
}