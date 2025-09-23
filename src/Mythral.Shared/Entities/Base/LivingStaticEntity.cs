using System.Numerics;
using Mythral.Shared.Contracts;
using Mythral.Shared.Types;

namespace Mythral.Shared.Entities.Base;

public abstract class LivingStaticEntity : StaticEntity, ILiving, ILifeCycle
{
    protected Health HealthComponent { get; }
    protected LifeCycle LifeCycleComponent { get; }

    public float Health => HealthComponent.Current;
    public float MaxHealth => HealthComponent.Max;
    public bool IsAlive => LifeCycleComponent.State == LifeState.Alive && HealthComponent.IsAlive;

    public LifeState State => LifeCycleComponent.State;
    public DateTime? ChangedAt => LifeCycleComponent.ChangedAt;

    protected LivingStaticEntity(float maxHealth)
    {
        HealthComponent = new Health(maxHealth);
        LifeCycleComponent = new LifeCycle();
    }

    protected LivingStaticEntity(Vector2 position, float maxHealth) : base(position)
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
        if (State == LifeState.Dead || State == LifeState.Ghost) return;
        HealthComponent.Heal(amount);
    }

    public void Kill()
    {
        HealthComponent.Kill();
        LifeCycleComponent.SetDead();
    }

    public void SetAlive() => LifeCycleComponent.SetAlive();
    public void SetDead()
    {
        LifeCycleComponent.SetDead();
        HealthComponent.Kill();
    }
    public void SetGhost()
    {
        LifeCycleComponent.SetGhost();
        HealthComponent.Kill();
    }
    public void SetRespawning() => LifeCycleComponent.SetRespawning();
}
