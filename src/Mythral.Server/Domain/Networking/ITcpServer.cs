namespace Mythral.Server.Domain.Networking;

using System.Collections.Concurrent;
using Mythral.Server.Domain.Game.Messages;

public interface ITcpServer
{
    void DrainRequests(ConcurrentQueue<ServerRequest> requestQueue);
    void Send<T>(T packet);
}
