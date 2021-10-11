using Google.Protobuf;
using System;
using System.IO;

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

                using (var ms = new MemoryStream())
                using (var input = new CodedInputStream(ms)) {
                    msg.MergeFrom(input);
                }

                return msg;
            }
            throw new NotImplementedException();
        }

        public byte[] Serialize(object value) {
            if (value is IMessage msg) {
                using (var ms = new MemoryStream())
                using (var output = new CodedOutputStream(ms)) {
                    msg.WriteTo(output);
                    return ms.ToArray();
                }
            }
            throw new NotImplementedException();
        }
    }
}
