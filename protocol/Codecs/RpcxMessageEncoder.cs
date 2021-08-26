using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using rpcx.net.Shared.Protocol;
using System.Text;
using static rpcx.net.Shared.Utils;

namespace rpcx.net.Shared.Codecs
{
    public class RpcxMessageEncoder : MessageToByteEncoder<Message>
    {
        protected override void Encode(IChannelHandlerContext context, Message message, IByteBuffer output)
        {
            var byHeader = message.Header.GetBytes();
            var byServicePath = Encoding.UTF8.GetBytes(message.ServicePath);
            var byServiceMethod = Encoding.UTF8.GetBytes(message.ServiceMethod);
            var byMetadata = message.Metadata.GetBytes();

            var compressor = GetCompressor(message.Header.CompressType);
            var byPayload = compressor is null ? message.Payload : compressor.Zip(message.Payload);

            output.WriteBytes(byHeader);
            output.WriteInt(4 + byServicePath.Length + 4 + byServiceMethod.Length + 4 + byMetadata.Length + 4 + byPayload.Length);
            output.WriteInt(byServicePath.Length);
            output.WriteBytes(byServicePath);
            output.WriteInt(byServiceMethod.Length);
            output.WriteBytes(byServiceMethod);
            output.WriteInt(byMetadata.Length);
            output.WriteBytes(byMetadata);
            output.WriteInt(byPayload.Length);
            output.WriteBytes(byPayload);
        }
    }
}
