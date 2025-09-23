namespace Mythral.Shared.Entities.Base;

public class Health
{
    public float Current { get; private set; }
    public float Max { get; private set; }

    public bool IsAlive => Current > 0f;

    public Health(float max, float? current = null)
    {
        if (max <= 0f) throw new ArgumentOutOfRangeException(nameof(max), "Max health must be > 0");
        Max = max;
        var initial = current ?? max;
        Current = Math.Clamp(initial, 0f, Max);
    }

    public void Damage(float amount)
    {
        if (amount <= 0f) return;
        Current = MathF.Max(0f, Current - amount);
    }

    public void Heal(float amount)
    {
        if (amount <= 0f) return;
        Current = MathF.Min(Max, Current + amount);
    }

    public void Kill()
    {
        Current = 0f;
    }
}
