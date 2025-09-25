using System.Collections.Concurrent;
using Mythral.Server.Domain.Game.Messages;

namespace Mythral.Server.Domain.Game;

public interface IGameWorld
{
    ConcurrentQueue<PlayerInput> InputQueue { get; }
    ConcurrentQueue<ServerRequest> RequestQueue { get; }
    IReadOnlyCollection<object> VisibleStates { get; }
    void Update(TimeSpan delta);
}
