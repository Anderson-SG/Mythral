using System.Numerics;
using Mythral.Shared.Entities;
using Mythral.Shared.Types;
using Xunit;

namespace Mythral.Shared.Tests;

public class WorldPropTests
{
    [Fact]
    public void WorldProp_Constructs_WithMetadata()
    {
        var prop = new WorldProp(new Vector2(2.2f, 3.3f), PropKind.Tree, isBlocking: true, variant: 2, rotationDegrees: 30f);
        Assert.Equal(PropKind.Tree, prop.Kind);
        Assert.True(prop.IsBlocking);
        Assert.Equal(2, prop.Variant);
        Assert.Equal(30f, prop.RotationDegrees);
        Assert.Equal(2, prop.Tile.X);
        Assert.Equal(3, prop.Tile.Y);
    }
}
