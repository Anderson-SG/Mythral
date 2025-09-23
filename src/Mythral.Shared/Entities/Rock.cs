using System.Numerics;
using Mythral.Shared.Entities.Base;

namespace Mythral.Shared.Entities;

public class Rock : StaticEntity
{
    public string Material { get; init; } = "Stone";

    public Rock()
    {
    }

    public Rock(Vector2 position, string? material = null)
        : base(position)
    {
        if (!string.IsNullOrWhiteSpace(material))
            Material = material;
    }
}
