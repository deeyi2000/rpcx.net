using System;

namespace rpcx.net.Shared.Codecs.Serializer
{
    public class JsonSerializer : ISerializer
    {
        private static JsonSerializer _default;
        public static JsonSerializer Default
        {
            get
            {
                if (_default is null)
                    _default = new JsonSerializer();
                return _default;
            }
        }

        public object Deserialize(Type type, ReadOnlyMemory<byte> bytes)
        {
            var json = System.Text.Encoding.UTF8.GetString(bytes.ToArray());
            return System.Text.Json.JsonSerializer.Deserialize(json, type);
        }
                    //System.Text.Json.JsonSerializer.Deserialize(bytes.Span, type);

        public byte[] Serialize(object value) =>
            System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(value);
    }
}
