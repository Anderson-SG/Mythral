using LiteNetLib;
using System.Collections.Concurrent;

namespace Mythral.Server.Domain.Sessions;

public sealed class Session
{
    public Guid SessionId { get; init; }
    public string UdpToken { get; internal set; } = string.Empty; // invalidated after bind
    public long TcpConnectionId { get; init; }
    public int AccountId { get; init; }
    public int? CharacterId { get; internal set; }
    public NetPeer? UdpPeer { get; internal set; }
    public System.Net.IPEndPoint? UdpRemoteEndPoint { get; internal set; }
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LastSeen { get; internal set; } = DateTimeOffset.UtcNow;
    public int LastRttMs { get; internal set; }
    public bool IsAuthenticated { get; internal set; } = true; // after login stub
    public bool IsUdpBound { get; internal set; }
    public ConcurrentQueue<Shared.Messages.PlayerInputMsg> InputQueue { get; } = new();
}
