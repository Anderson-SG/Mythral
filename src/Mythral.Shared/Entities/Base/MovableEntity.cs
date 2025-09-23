using System.Numerics;
using Mythral.Shared.Types;

namespace Mythral.Shared.Entities.Base;

public abstract class MovableEntity : Entity
{
    // World-space continuous position in tile units (1.0f = 1 tile)
    public Vector2 Position { get; protected set; } = Vector2.Zero;

    // Movement speed in tiles per second
    public float Speed { get; protected set; } = 3f;

    // Convenience: which tile the entity is currently over
    public Coordinate CurrentTile => Coordinate.FromWorld(Position);

    protected MovableEntity()
    {
    }

    protected MovableEntity(Vector2 initialPosition, float speed = 3f)
    {
        Position = initialPosition;
        Speed = speed;
    }

    // Set absolute world position
    public void SetPosition(Vector2 position)
    {
        Position = position;
    }

    // Move by a world-space delta (can be fractional, any direction)
    public void Move(Vector2 delta)
    {
        Position += delta;
    }

    // Move in a direction vector scaled by Speed and deltaTime (seconds)
    public void MoveTowards(Vector2 direction, float deltaTimeSeconds)
    {
        if (direction == Vector2.Zero || deltaTimeSeconds <= 0f)
            return;

        var dir = Vector2.Normalize(direction);
        Position += dir * Speed * deltaTimeSeconds;
    }
}
