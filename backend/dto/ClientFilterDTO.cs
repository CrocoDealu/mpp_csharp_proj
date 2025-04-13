namespace backend.dto;

public class ClientFilterDTO
{
    public string ClientName { get; set; }
    public string ClientAddress { get; set; }

    public ClientFilterDTO(string clientName, string clientAddress)
    {
        if (clientName == "")
            ClientName = null;
        else 
            ClientName = clientName;
        if (clientAddress == "")
            ClientAddress = null;
        else
        {
            ClientAddress = clientAddress;
        }
    }

    public override string ToString()
    {
        return "Client name: " + ClientName + " address: " + ClientAddress;
    }
}