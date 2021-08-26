using DotNetty.Transport.Channels;

namespace rpcx.net.Shared.Codecs
{
    public class RpcxClientCodec : CombinedChannelDuplexHandler<RpcxMessageDecoder, RpcxMessageEncoder>
    {
        public RpcxClientCodec()
        {
            Init(new RpcxMessageDecoder(), new RpcxMessageEncoder());
        }
    }
}
