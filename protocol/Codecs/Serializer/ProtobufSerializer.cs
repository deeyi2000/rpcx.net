using System;

namespace rpcx.net.Shared.Codecs.Serializer
{
    public class ProtobufSerializer : ISerializer
    {
        private static ProtobufSerializer _default;
        public static ProtobufSerializer Default
        {
            get
            {
                if (_default is null)
                    _default = new ProtobufSerializer();
                return _default;
            }
        }

        public object Deserialize(Type type, ReadOnlyMemory<byte> bytes)
        {
            throw new NotImplementedException();
        }

        public byte[] Serialize(object value)
        {
            throw new NotImplementedException();
        }
    }
}
