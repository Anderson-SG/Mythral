using System.Numerics;
using Mythral.Shared.Types;
using Xunit;

namespace Mythral.Shared.Tests;

public class CoordinateTests
{
    [Theory]
    [InlineData(0.0f, 0.0f, 0, 0)]
    [InlineData(0.1f, 0.9f, 0, 0)]
    [InlineData(1.0f, 1.0f, 1, 1)]
    [InlineData(1.9f, 1.1f, 1, 1)]
    [InlineData(-0.1f, -0.1f, -1, -1)]
    [InlineData(-1.0f, -1.0f, -1, -1)]
    [InlineData(-1.1f, -1.9f, -2, -2)]
    public void FromWorld_FloorsForNegatives(float wx, float wy, int ex, int ey)
    {
        var tile = Coordinate.FromWorld(new Vector2(wx, wy));
        Assert.Equal(ex, tile.X);
        Assert.Equal(ey, tile.Y);
    }

    [Fact]
    public void ToWorldCenter_ReturnsCenter()
    {
        var c = new Coordinate(3, 5);
        var center = c.ToWorldCenter();
        Assert.Equal(3.5f, center.X);
        Assert.Equal(5.5f, center.Y);
    }
}
