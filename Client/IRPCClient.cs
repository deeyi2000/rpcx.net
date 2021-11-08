using DotNetty.Transport.Channels;
using System.Net;
using System.Threading;
using rpcx.net.Shared.Protocol;
using System.Threading.Tasks;
using rpcx.net.Shared;

namespace rpcx.net.Client {
    public interface IRPCClient
	{
		bool Connect(string network, string address);
		Task<TReply> Go<TArgs, TReply>(IContext ctx, string servicePath, string serviceMethod, TArgs args, CancellationToken cancellationToken = default);
		Task SendRaw(Message message, CancellationToken cancellationToken = default);
		void Close();

		IChannel GetConn();
		EndPoint RemoteAddr { get; }
		bool IsClosing();
		bool IsShutdown();
	}
}
