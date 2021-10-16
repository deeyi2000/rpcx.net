using Google.Protobuf;
using System;
using System.Collections.Generic;
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

        private static Dictionary<Type, bool> _cacheType = new Dictionary<Type, bool>();
        private bool IsProtoContract(Type type) {
            lock (_cacheType) {
                if (_cacheType.ContainsKey(type)) {
                    return _cacheType[type];
                }
                var attrs = type.GetCustomAttributes(typeof(ProtoBuf.ProtoContractAttribute), false);
                bool exists = attrs.Length > 0;
                _cacheType[type] = exists;
                return exists;
            }
        }

        public object Deserialize(Type type, ReadOnlyMemory<byte> bytes) {
            if (type.IsAssignableFrom(typeof(IMessage))) {
                // google.protobuf
                var msg = (IMessage)Activator.CreateInstance(type);
                msg.MergeFrom(bytes.ToArray());

                return msg;
            } else if (IsProtoContract(type)) {
                // protobuf-net
                return ProtoBuf.Serializer.NonGeneric.Deserialize(type, bytes);
            }
            throw new NotImplementedException();
        }

        public byte[] Serialize(object value) {
            if (value is IMessage msg) {
                // google.protobuf
                return msg.ToByteArray();
            } else if (IsProtoContract(value.GetType())) {
                // protobuf-net
                using (var ms = new MemoryStream()) {
                    ProtoBuf.Serializer.NonGeneric.Serialize(ms, value);
                    return ms.ToArray();
                }
            }
            throw new NotImplementedException();
        }
    }
}
