using MessagePack;

namespace Mythral.Shared.Messages;

public enum Opcode
{
    LoginRequest = 1,
    LoginOk = 2,
    UdpHello = 3,
    StateSnapshot = 20,
    ChatMessage = 30
}

[MessagePackObject]
public sealed class LoginRequest
{
    [Key(0)] public string Username { get; init; } = string.Empty;
    [Key(1)] public string PasswordHash { get; init; } = string.Empty; // TODO: real auth hash
    [Key(2)] public string ClientVersion { get; init; } = "0.0.1"; // TODO: validate
}

[MessagePackObject]
public sealed class LoginOk
{
    [Key(0)] public Guid SessionId { get; init; }
    [Key(1)] public string UdpToken { get; init; } = string.Empty;
    [Key(2)] public int UdpPort { get; init; }
    [Key(3)] public int TickRate { get; init; }
}

[MessagePackObject]
public sealed class UdpHello
{
    [Key(0)] public Guid SessionId { get; init; }
    [Key(1)] public string UdpToken { get; init; } = string.Empty;
}

[MessagePackObject]
public sealed class EntityState
{
    [Key(0)] public int Id { get; init; }
    [Key(1)] public float X { get; init; }
    [Key(2)] public float Y { get; init; }
    [Key(3)] public byte Facing { get; init; }
    [Key(4)] public byte Anim { get; init; }
}

[MessagePackObject]
public sealed class StateSnapshot
{
    [Key(0)] public int Tick { get; init; }
    [Key(1)] public EntityState[] Entities { get; init; } = Array.Empty<EntityState>();
}

[MessagePackObject]
public sealed class ChatMessage
{
    [Key(0)] public Guid SessionId { get; init; }
    [Key(1)] public string Message { get; init; } = string.Empty; // TODO: sanitize
}
