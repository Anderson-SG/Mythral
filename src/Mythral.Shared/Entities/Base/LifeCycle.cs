using Mythral.Shared.Contracts;
using Mythral.Shared.Types;

namespace Mythral.Shared.Entities.Base;

public class LifeCycle : ILifeCycle
{
    public LifeState State { get; private set; } = LifeState.Alive;
    public DateTime? ChangedAt { get; private set; } = DateTime.UtcNow;

    public void SetAlive()
    {
        State = LifeState.Alive;
        ChangedAt = DateTime.UtcNow;
    }

    public void SetDead()
    {
        State = LifeState.Dead;
        ChangedAt = DateTime.UtcNow;
    }

    public void SetGhost()
    {
        State = LifeState.Ghost;
        ChangedAt = DateTime.UtcNow;
    }

    public void SetRespawning()
    {
        State = LifeState.Respawning;
        ChangedAt = DateTime.UtcNow;
    }
}
