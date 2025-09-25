namespace Mythral.Server.Domain.Game.Messages;

public sealed class PlayerInput
{
    public Guid PlayerId { get; init; }
    public string Command { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
