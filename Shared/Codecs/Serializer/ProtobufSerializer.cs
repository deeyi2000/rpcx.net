using Google.Protobuf;
using System;

namespace rpcx.net.Shared.Codecs.Serializer {
    public class ProtobufSerializer : ISerializer {
        private static ProtobufSerializer _default;
        public static ProtobufSerializer Default {
            get {
                if (_default is null)
                    _default = new ProtobufSerializer();
                return _default;
            }
        }

        public object Deserialize(Type type, ReadOnlyMemory<byte> bytes) {
            if (type.IsAssignableFrom(typeof(IMessage))) {
                var msg = (IMessage)Activator.CreateInstance(type);
                msg.MergeFrom(bytes.ToArray());

                return msg;
            }
            throw new NotImplementedException();
        }

        public byte[] Serialize(object value) {
            if (value is IMessage msg) {
                return msg.ToByteArray();
            }
            throw new NotImplementedException();
        }
    }
}
