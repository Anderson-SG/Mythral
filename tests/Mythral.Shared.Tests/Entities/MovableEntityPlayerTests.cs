using System.Numerics;
using Mythral.Shared.Entities;
using Xunit;

namespace Mythral.Shared.Tests;

public class MovableEntityPlayerTests
{
    [Fact]
    public void Move_AddsDelta_SubTile()
    {
        var p = new Player("A", new Vector2(1.25f, 2.75f), speed: 4f);
        p.Move(new Vector2(0.5f, -0.25f));
        Assert.Equal(new Vector2(1.75f, 2.5f), p.Position);
    }

    [Fact]
    public void MoveTowards_UsesSpeedAndDeltaTime()
    {
        var p = new Player("A", new Vector2(0f, 0f), speed: 2f);
        p.MoveTowards(new Vector2(1, 0), deltaTimeSeconds: 0.5f);
        Assert.Equal(new Vector2(1f, 0f), p.Position); // 2 tiles/s * 0.5s = 1 tile
    }

    [Theory]
    [InlineData(0.1f, 0.1f, 0, 0)]
    [InlineData(1.0f, 1.0f, 1, 1)]
    [InlineData(-0.1f, -0.1f, -1, -1)]
    public void CurrentTile_ReflectsPosition(float x, float y, int tx, int ty)
    {
        var p = new Player("A", new Vector2(x, y));
        var t = p.CurrentTile;
        Assert.Equal(tx, t.X);
        Assert.Equal(ty, t.Y);
    }
}
