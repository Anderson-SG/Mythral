using MessagePack;
using Mythral.Shared.Serialization;

namespace Mythral.Server.Serialization;

internal sealed class MessagePackSerializerAdapter : IMessageSerializer
{
    static MessagePackSerializerAdapter()
    {
        Resolvers.Register();
    }

    public byte[] Write<T>(T obj) => MessagePackSerializer.Serialize(obj);

    public T Read<T>(ReadOnlySpan<byte> buffer) => MessagePackSerializer.Deserialize<T>(new System.Buffers.ReadOnlySequence<byte>(buffer.ToArray()));

    public object Read(ReadOnlySpan<byte> buffer, Type type)
        => MessagePackSerializer.Deserialize(type, new System.Buffers.ReadOnlySequence<byte>(buffer.ToArray()))
           ?? throw new InvalidOperationException("Deserialização retornou nulo.");
}
