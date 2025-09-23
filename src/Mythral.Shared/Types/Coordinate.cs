using System.Numerics;

namespace Mythral.Shared.Types;

public class Coordinate
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public Coordinate(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }

    // Convert a world-space position (in tile units) to a tile coordinate, flooring toward -infinity.
    public static Coordinate FromWorld(Vector2 worldPosition)
    {
        var x = (int)MathF.Floor(worldPosition.X);
        var y = (int)MathF.Floor(worldPosition.Y);
        return new Coordinate(x, y);
    }

    // Top-left corner of this tile in world-space (tile units)
    public Vector2 ToWorldTopLeft() => new(X, Y);

    // Center of this tile in world-space (tile units)
    public Vector2 ToWorldCenter() => new(X + 0.5f, Y + 0.5f);
}