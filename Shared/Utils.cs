using rpcx.net.Shared.Codecs.Compressor;
using rpcx.net.Shared.Codecs.Serializer;
using System;
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

        public static string UrlEncode(this string s)
        {
            var sb = new StringBuilder();
            var coder = Encoding.UTF8;

            foreach(var c in s){
                if (char.IsWhiteSpace(c))
                    sb.Append('+');
                else if (char.IsLetterOrDigit(c) || ".-*_".Contains(c))
                    sb.Append(c);
                else {
                    foreach(var b in coder.GetBytes(new char[] { c }))
                        sb.Append(string.Format("%{0:X2}", b));
                }
            }
            return sb.ToString();
        }

        public static string UrlDecode(this string s)
        {
            var by = new List<byte>();
            var coder = Encoding.UTF8;

            var ss = s.GetEnumerator();
            while(ss.MoveNext())
            {
                var c = ss.Current;
                if (c.Equals('%'))
                {
                    if (!ss.MoveNext()) break;
                    var high = ss.Current;
                    if (!ss.MoveNext()) break;
                    var low = ss.Current;
                    by.Add(Convert.ToByte($"{high}{low}", 16));
                }
                else if (c.Equals("+"))
                    by.Add((byte)' ');
                else
                    by.Add((byte)c);
            }
            return  coder.GetString(by.ToArray());
        }
    }
}
