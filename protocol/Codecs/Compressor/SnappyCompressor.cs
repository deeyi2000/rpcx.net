namespace rpcx.net.Shared.Codecs.Compressor
{
    public class SnappyCompressor : ICompressor
    {
        private static SnappyCompressor _default;
        public static SnappyCompressor Default
        {
            get
            {
                if (_default is null)
                    _default = new SnappyCompressor();
                return _default;
            }
        }

        public byte[] Unzip(byte[] bytes) =>
            Snappy.SnappyCodec.Uncompress(bytes);

        public byte[] Zip(byte[] bytes) =>
            Snappy.SnappyCodec.Compress(bytes);
    }
}
