using System;

namespace rpcx.net.Shared.Codecs.Serializer
{
    public interface ISerializer
    {
        byte[] Serialize(object value);
        object Deserialize(Type type, ReadOnlyMemory<byte> bytes);
    }
}
