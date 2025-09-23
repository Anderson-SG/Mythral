namespace Mythral.Shared.Contracts;

public interface ILiving
{
    float Health { get; }
    float MaxHealth { get; }
    bool IsAlive { get; }

    void TakeDamage(float amount);
    void Heal(float amount);
    void Kill();
}
