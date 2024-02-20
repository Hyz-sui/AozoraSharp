using System.Collections.Generic;

namespace AozoraSharp;

public class AozoraClientManager
{
    public static AozoraClientManager Instance { get; } = new();

    public SessionKeeper SessionKeeper { get; } = new();

    private readonly HashSet<AozoraClient> allClients = new(16);

    public void AddClient(AozoraClient client)
    {
        allClients.Add(client);
    }
    public void RemoveClient(AozoraClient client)
    {
        allClients.Remove(client);
    }
}
