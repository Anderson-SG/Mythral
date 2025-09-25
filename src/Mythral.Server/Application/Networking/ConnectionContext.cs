using System.Net.Sockets;
using Mythral.Server.Domain.Networking;

namespace Mythral.Server.Application.Networking;

internal sealed class ConnectionContext : IConnectionContext
{
    public Guid Id { get; } = Guid.NewGuid();
    public TcpClient Client { get; }
    public bool IsAuthenticated { get; set; }
    public int AccountId { get; set; }
    public DateTimeOffset ConnectedAt { get; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LastSeen { get; set; } = DateTimeOffset.UtcNow;

    public ConnectionContext(TcpClient client)
    {
        Client = client;
    }
}
