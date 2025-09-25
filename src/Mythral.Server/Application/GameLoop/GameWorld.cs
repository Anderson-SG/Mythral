using System.Collections.Concurrent;
using Mythral.Server.Domain.Game;
using Mythral.Server.Domain.Game.Messages;

namespace Mythral.Server.Application.GameLoop;

public sealed class GameWorld : IGameWorld
{
    private readonly List<object> _visibleStates = new();

    public ConcurrentQueue<PlayerInput> InputQueue { get; } = new();
    public ConcurrentQueue<ServerRequest> RequestQueue { get; } = new();
    public IReadOnlyCollection<object> VisibleStates => _visibleStates;

    public void Update(TimeSpan delta)
    {
        while (InputQueue.TryDequeue(out var input))
        {
        }
        while (RequestQueue.TryDequeue(out var req))
        {
        }
        _visibleStates.Clear();
    }
}
