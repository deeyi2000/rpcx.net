using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using rpcx.net.Shared.Protocol;
using System.Collections.Generic;
using System.Text;
using static rpcx.net.Shared.Utils;

namespace rpcx.net.Shared.Codecs
{
    public class RpcxMessageDecoder : LengthFieldBasedFrameDecoder
    {
        public RpcxMessageDecoder(int maxFrameLength = 10240) :
            base(maxFrameLength, 12, 4) { }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            if (base.Decode(context, input) is IByteBuffer buf)
            {
                int nLen;

                var byHeader = new byte[16];
                buf.ReadBytes(byHeader, 0, byHeader.Length);
                var header = Header.FromBytes(byHeader);

                string servicePath = null;
                if (0 != (nLen = buf.ReadInt()))
                    servicePath = buf.ReadString(nLen, Encoding.UTF8);

                string serviceMethod = null;
                if (0 != (nLen = buf.ReadInt()))
                    serviceMethod = buf.ReadString(nLen, Encoding.UTF8);

                Dictionary<string, string> metadata = null;
                if (0 != (nLen = buf.ReadInt()))
                {
                    buf.SetReaderIndex(buf.ReaderIndex + nLen);  //TODO: read Metadata
                }
                
                byte[] payload = null;
                if (0 != (nLen = buf.ReadInt()))
                {
                    payload = new byte[nLen];
                    buf.ReadBytes(payload, 0, nLen);
                    var compressor = GetCompressor(header.CompressType);
                    if (compressor != null)
                        payload = compressor.Unzip(payload);
                }

                output.Add(new Message(header) {
                    ServicePath = servicePath,
                    ServiceMethod = serviceMethod,
                    Metadata = metadata,
                    Payload = payload,
                });
            }
        }
    }
}
