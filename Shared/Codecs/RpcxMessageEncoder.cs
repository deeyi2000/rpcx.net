using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using rpcx.net.Shared.Protocol;
using System.Collections.Generic;
using System.Text;
using static rpcx.net.Shared.Utils;

namespace rpcx.net.Shared.Codecs {
    public class RpcxMessageEncoder : MessageToByteEncoder<Message> {
        protected override void Encode(IChannelHandlerContext context, Message message, IByteBuffer output) {
            var byHeader = message.Header.GetBytes();
            var byServicePath = Encoding.UTF8.GetBytes(message.ServicePath);
            var byServiceMethod = Encoding.UTF8.GetBytes(message.ServiceMethod);
            //var byMetadata = message.Metadata.GetBytes();
            var bufMetadata = context.Allocator.Buffer();
            var nLenMetadata = EncodeMetadata(bufMetadata, message.Context, Encoding.UTF8);

            var compressor = GetCompressor(message.Header.CompressType);
            var byPayload = compressor is null ? message.Payload : compressor.Zip(message.Payload);

            output.WriteBytes(byHeader);
            output.WriteInt(4 + byServicePath.Length + 4 + byServiceMethod.Length + 4 + nLenMetadata + 4 + byPayload.Length);
            output.WriteInt(byServicePath.Length);
            output.WriteBytes(byServicePath);
            output.WriteInt(byServiceMethod.Length);
            output.WriteBytes(byServiceMethod);
            //output.WriteInt(byMetadata.Length);
            //output.WriteBytes(byMetadata);
            output.WriteInt(nLenMetadata);
            output.WriteBytes(bufMetadata);
            output.WriteInt(byPayload.Length);
            output.WriteBytes(byPayload);
        }

        protected int EncodeMetadata(IByteBuffer buf, IContext ctx, Encoding encoding) {
            if (ctx == null) { return 0; }

            // todo: 如果ReqMetaDataKey对应的类型不是IDictionary<string, string>, 应当怎样处理
            var metadata = ctx.Value(Context.ReqMetaDataKey) as IDictionary<string, string>;
            if (metadata == null) { return 0; }

            var startIdx = buf.WriterIndex;

            byte[] k, v;
            foreach (var kv in metadata) {
                k = encoding.GetBytes(kv.Key);
                v = encoding.GetBytes(kv.Value);
                buf.WriteInt(k.Length);
                buf.WriteBytes(k);
                buf.WriteInt(v.Length);
                buf.WriteBytes(v);
            }
            return buf.WriterIndex - startIdx;
        }
    }
}
