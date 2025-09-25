namespace Mythral.Server.Serialization;

public interface IMessageSerializer
{
    byte[] Write<T>(T obj);
    byte[] Write(Type type, object obj);
    T Read<T>(ReadOnlySpan<byte> buffer);
    object Read(ReadOnlySpan<byte> buffer, Type type);
}
