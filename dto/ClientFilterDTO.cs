namespace ConsoleApp1.dto;

public class ClientFilterDTO
{
    public string ClientName { get; set; }
    public string ClientAddress { get; set; }

    public ClientFilterDTO(string clientName, string clientAddress)
    {
        ClientName = clientName;
        ClientAddress = clientAddress;
    }
}