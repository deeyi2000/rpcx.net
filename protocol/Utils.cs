using rpcx.net.Shared.Codecs.Compressor;
using rpcx.net.Shared.Codecs.Serializer;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static rpcx.net.Shared.Protocol.Header;

namespace rpcx.net.Shared
{
    public static class Utils
    {
        public static readonly ISerializer[] Serializers = {
            ByteSerializer.Default,
            JsonSerializer.Default,
            ProtobufSerializer.Default,
            MessagePackSerializer.Default,
        };

        public static ISerializer GetSerializer(eType type) =>
            Serializers[((ushort)type & (ushort)eTypeMask.serializeType) >> 4];

        public static readonly ICompressor[] Compressors = {
            null,
            GZipCompressor.Default,
            SnappyCompressor.Default,
        };

        public static ICompressor GetCompressor(eType type) =>
            Compressors[((ushort)type & (ushort)eTypeMask.compressType) >> 10];

        public static byte[] GetBigEndianBytes(this long l)
        {
            return new byte[]{
                (byte)(l >> 24),
                (byte)(l >> 16),
                (byte)(l >> 8),
                (byte)l,
            };
        }

        public static byte[] GetBytes(this Dictionary<string, string> dic)
        {
            if (dic is null || dic.Count == 0) return new byte[] { };

            return dic.Select(kv =>
            {
                var k = Encoding.UTF8.GetBytes(kv.Key);
                var v = Encoding.UTF8.GetBytes(kv.Value);
                return k.LongLength.GetBigEndianBytes()
                        .Concat(k)
                        .Concat(v.LongLength.GetBigEndianBytes())
                        .Concat(v);
            }).Aggregate((a, b) => a.Concat(b)).ToArray();
        }
    }
}
