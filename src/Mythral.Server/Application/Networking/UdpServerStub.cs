using System.Collections.Concurrent;
using Mythral.Server.Domain.Game.Messages;
using Mythral.Server.Domain.Networking;

namespace Mythral.Server.Application.Networking;

public sealed class UdpServerStub : IUdpServer
{
    public void DrainInputs(ConcurrentQueue<PlayerInput> inputQueue)
    {
    }

    public void BroadcastSnapshots(IReadOnlyCollection<object> snapshots)
    {
    }
}
