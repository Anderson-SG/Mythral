using System.Numerics;
using Mythral.Shared.Entities.Base;

namespace Mythral.Shared.Entities;

// Training dummy: static but has health and can be damaged
public class Dummy : LivingStaticEntity
{
    public Dummy(float maxHealth = 500f) : base(maxHealth) { }
    public Dummy(Vector2 position, float maxHealth = 500f) : base(position, maxHealth) { }
}
