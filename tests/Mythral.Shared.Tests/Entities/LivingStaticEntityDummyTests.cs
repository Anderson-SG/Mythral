using System.Numerics;
using Mythral.Shared.Entities;
using Xunit;

namespace Mythral.Shared.Tests;

public class LivingStaticEntityDummyTests
{
    [Fact]
    public void Dummy_TakesDamage_AndDies()
    {
        var d = new Dummy(new Vector2(1,1), maxHealth: 5f);
        d.TakeDamage(3f);
        Assert.True(d.IsAlive);
        d.TakeDamage(2f);
        Assert.False(d.IsAlive);
    }
}
