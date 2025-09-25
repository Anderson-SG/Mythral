namespace Mythral.Server.Domain.Networking;

using Mythral.Server.Domain.Game.Messages;
using System.Collections.Concurrent;

public interface IUdpServer
{
    void DrainInputs(ConcurrentQueue<PlayerInput> inputQueue);
    void BroadcastSnapshots(IReadOnlyCollection<object> snapshots);
}
