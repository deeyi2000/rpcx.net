using System;

namespace rpcx.net.Shared.Codecs.Serializer {
    public class JsonSerializer : ISerializer {
        private static JsonSerializer _default;
        public static JsonSerializer Default {
            get {
                if (_default is null)
                    _default = new JsonSerializer();
                return _default;
            }
        }

        public object Deserialize(Type type, ReadOnlyMemory<byte> bytes) {
            return Utf8Json.JsonSerializer.NonGeneric.Deserialize(type, bytes.ToArray());
        }


        // 替换成 Utf8Json 序列化
        // 因为 在序列化 Message WithMetadata 时, System.Text.Json 会崩溃

        public byte[] Serialize(object value) => Utf8Json.JsonSerializer.Serialize(value);
    }
}
