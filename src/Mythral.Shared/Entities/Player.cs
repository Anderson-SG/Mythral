using System.Numerics;
using Mythral.Shared.Entities.Base;

namespace Mythral.Shared.Entities;

public class Player : LivingMovableEntity
{
    public string Name { get; private set; }

    public Player(string name) : base(maxHealth: 100)
    {
        Name = name;
    }

    public Player(string name, Vector2 initialPosition, float speed = 3f, int maxHealth = 100)
        : base(initialPosition, speed, maxHealth)
    {
        Name = name;
    }
}