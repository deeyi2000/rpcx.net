using System;

namespace rpcx.net.Shared.Codecs.Compressor
{
    public interface ICompressor
    {
        byte[] Zip(byte[] bytes);
        byte[] Unzip(byte[] bytes);
    }
}
