using MessagePack;

namespace Mythral.Shared.Messages;

[MessagePackObject]
public sealed class PingPacket
{
    [Key(0)] public long Timestamp { get; init; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}

[MessagePackObject]
public sealed class PongPacket
{
    [Key(0)] public long EchoTimestamp { get; init; }
    [Key(1)] public long ServerTimestamp { get; init; }
}
