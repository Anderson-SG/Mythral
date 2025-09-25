namespace Mythral.Server.Domain.Game.Messages;

public sealed class ServerRequest
{
    public Guid ConnectionId { get; init; }
    // Texto legado (ex: comandos linha-a-linha). Pode ficar vazio quando for binário.
    public string Payload { get; init; } = string.Empty;
    // Novo protocolo binário
    public ushort Opcode { get; init; }
    public ReadOnlyMemory<byte> BinaryPayload { get; init; } = ReadOnlyMemory<byte>.Empty;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
