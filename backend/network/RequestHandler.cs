using backend.dto;
using backend.model;
using backend.service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace backend.network;


public class RequestHandler
{
    private SportsTicketManagementService _service;

    public RequestHandler(SportsTicketManagementService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }
    public void HandleRequest(string requestString, BackendClient client)
    {
        try
        {
            var request = JObject.Parse(requestString);
            var response = new JObject();

            if (request.ContainsKey("messageId"))
            {
                response["messageId"] = request["messageId"];
            }

            var type = request["type"]?.ToString();
            switch (type)
            {
                case "LOGIN":
                    HandleLogin(request, client, response);
                    break;
                case "SAVE_TICKET":
                    HandleSaveTicket(request, client, response);
                    break;
                case "GET_GAMES":
                    HandleGetGames(client, response);
                    break;
                case "GET_TICKETS":
                    var filter = GetClientFilterDTO(request);
                    HandleGetTickets(client, filter, response);
                    break;
                case "ERROR":
                    HandleError();
                    break;
                case "LOGOUT":
                    HandleLogout(client);
                    break;
                default:
                    Console.WriteLine($"Unknown request type: {type}");
                    break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error handling request: {e.Message}");
        }
    }

    private ClientFilterDTO GetClientFilterDTO(JObject request)
    {
        if (request.ContainsKey("payload"))
        {
            return ParseFilter(request["payload"] as JObject);
        }
        return null;
    }

    private ClientFilterDTO ParseFilter(JObject jsonObject)
    {
        var username = jsonObject?["username"]?.ToString() ?? null;
        var address = jsonObject?["address"]?.ToString() ?? null;
        return new ClientFilterDTO(username, address);
    }

    private void HandleLogin(JObject request, BackendClient client, JObject response)
    {
        var payload = request["payload"] as JObject;
        var username = payload?["username"]?.ToString();
        var password = payload?["password"]?.ToString();

        Console.WriteLine("Attempting login");

        var cashier = _service.GetCashierByUsername(username);
        if (cashier == null)
        {
            SendFailedLoginResponse(client, "USER_NOT_FOUND", response);
        }
        else if (cashier.Password != password)
        {
            SendFailedLoginResponse(client, "INCORRECT_PASSWORD", cashier.Id, response);
        }
        else
        {
            SendSuccessfulLoginResponse(cashier, client, response);
        }
    }

    private void SendSuccessfulLoginResponse(Cashier cashier, BackendClient client, JObject response)
    {
        response["type"] = "LOGIN_RESPONSE";

        var payload = new JObject
        {
            ["token"] = cashier.Id.ToString(),
            ["id"] = cashier.Id,
            ["username"] = cashier.Username,
            ["name"] = cashier.Name
        };

        response["payload"] = payload;
        String responseString = response.ToString(Newtonsoft.Json.Formatting.None);
        client.Send(responseString);
    }

    private void SendFailedLoginResponse(BackendClient client, string reason, JObject response)
    {
        response["type"] = "LOGIN_RESPONSE";

        var payload = new JObject
        {
            ["token"] = string.Empty,
            ["id"] = -1,
            ["username"] = string.Empty,
            ["name"] = string.Empty,
            ["reason"] = reason
        };

        response["payload"] = payload;
        String responseString = response.ToString(Newtonsoft.Json.Formatting.None);
        client.Send(responseString);
    }

    private void SendFailedLoginResponse(BackendClient client, string reason, int userId, JObject response)
    {
        response["type"] = "LOGIN_RESPONSE";

        var payload = new JObject
        {
            ["token"] = string.Empty,
            ["id"] = userId,
            ["username"] = string.Empty,
            ["name"] = string.Empty,
            ["reason"] = reason
        };

        response["payload"] = payload;
        String responseString = response.ToString(Newtonsoft.Json.Formatting.None);
        client.Send(responseString);
    }

    private void HandleSaveTicket(JObject request, BackendClient client, JObject response)
    {
        var payload = request["payload"] as JObject;
        var gameId = payload?["gameId"]?.ToObject<int>() ?? 0;
        var clientName = payload?["clientName"]?.ToString();
        var clientAddress = payload?["clientAddress"]?.ToString();
        var cashierId = payload?["cashierId"]?.ToObject<int>() ?? 0;
        var noOfSeats = payload?["noOfSeats"]?.ToObject<int>() ?? 0;

        var game = _service.GetGameById(gameId);
        if (game != null)
        {
            var ticket = new Ticket(0, game, clientName, clientAddress, new Cashier(cashierId), noOfSeats);
            _service.SaveTicket(ticket);

            response["type"] = "SAVE_TICKET_RESPONSE";
            var responsePayload = new JObject
            {
                ["message"] = "Ticket saved successfully"
            };
            response["payload"] = responsePayload;
            String responseString = response.ToString(Newtonsoft.Json.Formatting.None);
            client.Send(responseString);
        }
    }

    private void HandleGetGames(BackendClient client, JObject response)
    {
        var games = _service.GetAllGames().ToList();
        var jsonString = JsonConvert.SerializeObject(games, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        });

        response["type"] = "GET_GAMES_RESPONSE";
        var payload = new JObject
        {
            ["result"] = jsonString
        };
        response["payload"] = payload;
        String responseString = response.ToString(Newtonsoft.Json.Formatting.None);
        client.Send(responseString);
    }

    private void HandleGetTickets(BackendClient client, ClientFilterDTO filter, JObject response)
    {
        var tickets = _service.GetTicketsForClient(filter).ToList();
        var jsonString = JsonConvert.SerializeObject(tickets, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        });
        response["type"] = "GET_TICKETS_RESPONSE";
        var payload = new JObject
        {
            ["result"] = jsonString
        };
        response["payload"] = payload;
        String responseString = response.ToString(Newtonsoft.Json.Formatting.None);
        client.Send(responseString);
    }

    private void HandleError()
    {
        Console.WriteLine("Oops, got an error");
    }

    private void HandleLogout(BackendClient client)
    {
        _service.LogoutClient(client);
    }
}