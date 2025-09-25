using System.Net.Sockets;

namespace Mythral.Server.Domain.Networking;

public interface IConnectionContext
{
    Guid Id { get; }
    TcpClient Client { get; }
    bool IsAuthenticated { get; set; }
    int AccountId { get; set; }
    DateTimeOffset ConnectedAt { get; }
    DateTimeOffset LastSeen { get; set; }
}
