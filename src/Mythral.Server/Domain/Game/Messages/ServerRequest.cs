namespace Mythral.Server.Domain.Game.Messages;

public sealed class ServerRequest
{
    public Guid ConnectionId { get; init; }
    public string Payload { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
