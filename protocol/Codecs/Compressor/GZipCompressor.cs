using System;
using System.IO;
using System.IO.Compression;

namespace rpcx.net.Shared.Codecs.Compressor
{
    public class GZipCompressor : ICompressor
    {
        private static GZipCompressor _default;
        public static GZipCompressor Default
        {
            get
            {
                if (_default is null)
                    _default = new GZipCompressor();
                return _default;
            }
        }

        public byte[] Unzip(byte[] bytes)
        {
            using (var compressStream = new MemoryStream(bytes))
            using (var zipStream = new GZipStream(compressStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                resultStream.Flush();
                return resultStream.ToArray();
            }
        }

        public byte[] Zip(byte[] bytes)
        {
            using (var compressStream = new MemoryStream())
            using (var zipStream = new GZipStream(compressStream, CompressionMode.Compress))
            {
                zipStream.Write(bytes, 0, bytes.Length);
                zipStream.Flush();
                return compressStream.ToArray();
            }
        }
    }
}
