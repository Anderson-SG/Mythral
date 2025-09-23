using System.Numerics;
using Mythral.Shared.Entities.Base;
using Mythral.Shared.Types;

namespace Mythral.Shared.Entities;

public class WorldProp : StaticEntity
{
    public PropKind Kind { get; init; } = PropKind.Unknown;

    // Whether this decoration blocks movement (e.g., big rock, tree trunk).
    public bool IsBlocking { get; init; } = false;

    // Optional rotation for rendering (degrees)
    public float RotationDegrees { get; init; } = 0f;

    // Variant index for spritesheets/tilesets (e.g., different bush shapes)
    public int Variant { get; init; } = 0;

    public WorldProp() {}

    public WorldProp(Vector2 position, PropKind kind, bool isBlocking = false, int variant = 0, float rotationDegrees = 0f)
        : base(position)
    {
        Kind = kind;
        IsBlocking = isBlocking;
        Variant = variant;
        RotationDegrees = rotationDegrees;
    }
}
