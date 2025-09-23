using Mythral.Shared.Types;

namespace Mythral.Shared.Contracts;

public interface ILifeCycle
{
    LifeState State { get; }
    DateTime? ChangedAt { get; }

    void SetAlive();
    void SetDead();
    void SetGhost();
    void SetRespawning();
}
