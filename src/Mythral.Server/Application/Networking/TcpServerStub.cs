using System.Collections.Concurrent;
using Mythral.Server.Domain.Game.Messages;
using Mythral.Server.Domain.Networking;

namespace Mythral.Server.Application.Networking;

public sealed class TcpServerStub : ITcpServer
{
    public void DrainRequests(ConcurrentQueue<ServerRequest> requestQueue)
    {
    }
}
