using MessagePack;
using MessagePack.Resolvers;

namespace Mythral.Shared.Serialization;

public static class Resolvers
{
    private static bool _registered;
    private static readonly object _lock = new();

    public static void Register()
    {
        if (_registered) return;
        lock (_lock)
        {
            if (_registered) return;
            var resolver = CompositeResolver.Create(
                // TODO: add generated resolvers when SourceGen enabled
                StandardResolverAllowPrivate.Instance,
                StandardResolver.Instance
            );
            var options = MessagePackSerializerOptions.Standard
                .WithResolver(resolver)
                .WithCompression(MessagePackCompression.Lz4BlockArray);
            MessagePackSerializer.DefaultOptions = options;
            _registered = true;
        }
    }
}
