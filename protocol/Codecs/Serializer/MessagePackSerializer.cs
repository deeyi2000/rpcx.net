using System;

namespace rpcx.net.Shared.Codecs.Serializer
{
    public class MessagePackSerializer : ISerializer
    {
        private static MessagePackSerializer _default;
        public static MessagePackSerializer Default
        {
            get
            {
                if (_default is null)
                    _default = new MessagePackSerializer();
                return _default;
            }
        }

        public object Deserialize(Type type, ReadOnlyMemory<byte> bytes) =>
            MessagePack.MessagePackSerializer.Deserialize(type, bytes, MessagePack.Resolvers.ContractlessStandardResolver.Options);

        public byte[] Serialize(object value) =>
            MessagePack.MessagePackSerializer.Serialize(value, MessagePack.Resolvers.ContractlessStandardResolver.Options);
    }
}
