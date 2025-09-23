using System.Numerics;
using Mythral.Shared.Entities;
using Mythral.Shared.Entities.Base;
using Mythral.Shared.Types;
using Xunit;

namespace Mythral.Shared.Tests;

public class HealthAndLifeCycleTests
{
    [Fact]
    public void Health_DamageHeal_Kill_Clamped()
    {
        var h = new Health(100f);
        Assert.True(h.IsAlive);
        Assert.Equal(100f, h.Current);

        h.Damage(30f);
        Assert.Equal(70f, h.Current);

        h.Damage(1000f);
        Assert.Equal(0f, h.Current);
        Assert.False(h.IsAlive);

        h.Heal(10f);
        Assert.Equal(10f, h.Current);

        h.Heal(1000f);
        Assert.Equal(100f, h.Current);

        h.Kill();
        Assert.Equal(0f, h.Current);
    }

    [Fact]
    public void LivingMovable_Kill_SetsDead()
    {
        var p = new Player("Hero", new Vector2(0,0), speed: 3f, maxHealth: 10f);
        p.Kill();
        Assert.Equal(LifeState.Dead, p.State);
        Assert.False(p.IsAlive);
    }

    [Fact]
    public void LivingMovable_TakeDamage_ToZero_SetsDead()
    {
        var p = new Player("Hero", new Vector2(0,0), speed: 3f, maxHealth: 10f);
        p.TakeDamage(5f);
        Assert.True(p.IsAlive);
        p.TakeDamage(5f);
        Assert.Equal(LifeState.Dead, p.State);
        Assert.False(p.IsAlive);
    }

    [Fact]
    public void Respawn_RestoresHealthAndSetsAlive()
    {
        var p = new Player("Hero", new Vector2(0,0), speed: 3f, maxHealth: 20f);
        p.Kill();
        p.Respawn(new Vector2(5,5), healthFraction: 0.5f);
        Assert.Equal(LifeState.Alive, p.State);
        Assert.Equal(new Vector2(5,5), p.Position);
        Assert.Equal(10f, p.Health);
    }

    [Fact]
    public void Ghost_IgnoresDamage()
    {
        var p = new Player("Hero", new Vector2(0,0), speed: 3f, maxHealth: 20f);
        p.SetGhost();
        p.TakeDamage(100f);
        // Health stays at max because we set ghost and kill health to zero in SetGhost(); but damage is ignored.
        Assert.Equal(LifeState.Ghost, p.State);
        Assert.Equal(0f, p.Health);
    }
}
