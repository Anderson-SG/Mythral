using System.Numerics;
using Mythral.Shared.Types;

namespace Mythral.Shared.Entities.Base;

public abstract class LivingMovableEntity : MovableEntity, Mythral.Shared.Contracts.ILiving, Mythral.Shared.Contracts.ILifeCycle
{
    protected Health HealthComponent { get; }
    protected LifeCycle LifeCycleComponent { get; }

    public float Health => HealthComponent.Current;
    public float MaxHealth => HealthComponent.Max;
    public bool IsAlive => LifeCycleComponent.State == LifeState.Alive && HealthComponent.IsAlive;

    public LifeState State => LifeCycleComponent.State;
    public DateTime? ChangedAt => LifeCycleComponent.ChangedAt;

    protected LivingMovableEntity(float maxHealth)
    {
        HealthComponent = new Health(maxHealth);
        LifeCycleComponent = new LifeCycle();
    }

    protected LivingMovableEntity(Vector2 initialPosition, float speed, float maxHealth)
        : base(initialPosition, speed)
    {
        HealthComponent = new Health(maxHealth);
        LifeCycleComponent = new LifeCycle();
    }

    public void TakeDamage(float amount)
    {
        if (State == LifeState.Ghost || State == LifeState.Respawning) return;

        HealthComponent.Damage(amount);

        if (HealthComponent.Current <= 0f)
        {
            LifeCycleComponent.SetDead();
        }
    }

    public void Heal(float amount)
    {
        if (State == LifeState.Dead || State == LifeState.Ghost) return; // prevent healing when not alive/respawning
        
        HealthComponent.Heal(amount);
    }

    public void Kill()
    {
        HealthComponent.Kill();
        LifeCycleComponent.SetDead();
    }

    // ILifeCycle pass-throughs
    public void SetAlive() => LifeCycleComponent.SetAlive();
    public void SetDead()
    {
        LifeCycleComponent.SetDead();
        HealthComponent.Kill();
    }
    public void SetGhost()
    {
        LifeCycleComponent.SetGhost();
        // By design, ghosts have zero health
        HealthComponent.Kill();
    }
    public void SetRespawning() => LifeCycleComponent.SetRespawning();

    // Convenience: perform a respawn with position and health restoration
    public void Respawn(Vector2 atPosition, float healthFraction = 1f)
    {
        SetRespawning();
        SetPosition(atPosition);
        var restore = MathF.Max(0f, MathF.Min(1f, healthFraction));
        // Directly heal from 0 to target fraction of Max
        var target = MaxHealth * restore;
        if (target > Health)
        {
            HealthComponent.Heal(target - Health);
        }
        else if (target < Health)
        {
            HealthComponent.Damage(Health - target);
        }
        SetAlive();
    }
}
