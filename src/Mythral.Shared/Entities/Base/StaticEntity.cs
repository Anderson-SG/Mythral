using System.Numerics;
using Mythral.Shared.Types;

namespace Mythral.Shared.Entities.Base;

public abstract class StaticEntity : Entity
{
    // World-space position in tile units
    public Vector2 Position { get; init; }

    public Coordinate Tile => Coordinate.FromWorld(Position);

    protected StaticEntity()
    {
    }

    protected StaticEntity(Vector2 position)
    {
        Position = position;
    }
}
