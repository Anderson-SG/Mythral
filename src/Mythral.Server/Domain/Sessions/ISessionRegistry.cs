using LiteNetLib;
using System.Net;

namespace Mythral.Server.Domain.Sessions;

public interface ISessionRegistry
{
    Session Create(long tcpId, int accountId, string udpToken);
    bool TryGet(Guid sessionId, out Session session);
    bool TryGetByTcp(long tcpId, out Session session);
    bool TryGetByUdp(IPEndPoint endPoint, out Session session);
    bool TryBindUdp(Guid sessionId, string udpToken, NetPeer peer);
    void Touch(Guid sessionId, int? rttMs = null);
    void SetCharacter(Guid sessionId, int characterId);
    bool Remove(Guid sessionId, string reason, string? details = null);
    IReadOnlyCollection<Session> Snapshot();
    string NewUdpToken(int bytes = 16);
}
