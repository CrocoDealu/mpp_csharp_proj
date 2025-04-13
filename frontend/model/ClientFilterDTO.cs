namespace frontend.model;

public class ClientFilterDTO
{
    public string ClientName { get; set; }
    public string ClientAddress { get; set; }

    public ClientFilterDTO(string clientName, string clientAddress)
    {
        ClientName = clientName;
        ClientAddress = clientAddress;
    }

    public override string ToString()
    {
        return "Client name: " + ClientName + " address: " + ClientAddress;
    }
}