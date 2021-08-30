﻿using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using rpcx.net.Client.Generator;
using rpcx.net.Shared.Codecs;
using rpcx.net.Shared.Protocol;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using static rpcx.net.Shared.Utils;

namespace rpcx.net.Client
{
    public class Option
    {
        public string Group { get; set; }
        public int Retries { get; set; }
        public Header.eType Types { get; set; }

        public static Option Default
        {
            get => new Option() {
                Retries = 3,
                Types = Header.eType.MessagePack
            };
        }
    }

    public class Client : SimpleChannelInboundHandler<Message>, IRPCClient
    {
        protected static IEventLoopGroup group;
        protected static IEventLoopGroup Group
        {
            get
            {
                if (group is null)
                    group = new MultithreadEventLoopGroup();
                return group;
            }
        }

        protected Option _option;
        protected IChannel _chan;
        protected ConcurrentDictionary<long, TaskCompletionSource> _pending;

        public IIdGenerator<long> IdGen { get; set; } = IncIdGenerator.Default;

        public delegate void ServerMessageHandler(object sender, Message msg);
        public event ServerMessageHandler OnServerMessage;

        public Client(Option option = null)
        {
            _option = option ?? Option.Default;
            _pending = new ConcurrentDictionary<long, TaskCompletionSource>();
        }

        #region IRPCClient
        public IChannel GetConn() => _chan;
        public EndPoint RemoteAddr => _chan?.RemoteAddress;
        public bool IsClosing() => !(_chan?.Active).GetValueOrDefault(true);
        public bool IsShutdown() => _chan is null;

        public bool Connect(string network, string address)
        {
            var bootstrap = new Bootstrap();
            bootstrap.Group(Group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<ISocketChannel>(chan =>
                {
                    var pipe = chan.Pipeline;
                    pipe.AddLast(
                        new RpcxClientCodec(),
                        this
                    );
                }));
            var addr = address.Split(new char[] { ':' });
            _chan = bootstrap.ConnectAsync(addr[0], int.Parse(addr[1])).Result;
            return true;
        }

        public async Task<TReply> Go<TArgs, TReply>(string servicePath, string serviceMethod, TArgs args, CancellationToken cancellationToken = default)
        {
            var header = Header.NewRequest(_option.Types);
            var msg = new Message(header) {
                ServicePath = servicePath,
                ServiceMethod = serviceMethod,
                Metadata = args is WithMetadata a ? a._metadata : null,
                Payload = GetSerializer(header.SerializeType).Serialize(args),
            };

            msg.Header.Seq = IdGen.Next();
            var tcs = new TaskCompletionSource(typeof(TReply));
            _pending.TryAdd(msg.Header.Seq, tcs);
            _ = _chan.WriteAndFlushAsync(msg);
            return (TReply)await tcs.Task.ConfigureAwait(false);
        }

        public Task SendRaw(Message message, CancellationToken cancellationToken = default)
        {
            message.Header.Seq = IdGen.Next();
            var tcs = new TaskCompletionSource();
            _pending.TryAdd(message.Header.Seq, tcs);
            _ = _chan.WriteAndFlushAsync(message);
            return tcs.Task;
        }

        public void Close()
        {
            var chan = Interlocked.Exchange(ref _chan, null);
            chan.CloseAsync();
        }
        #endregion

        #region SimpleChannelInboundHandler<Message>
        protected override void ChannelRead0(IChannelHandlerContext ctx, Message msg)
        {
            if(msg.Header.MessageStatusType == Header.eType.Error)
            {
            }
            if(msg.Header.MessageType == Header.eType.Request && !msg.Header.IsHeartbeat && msg.Header.IsOneWay)
            {
                //ServerMessage
                OnServerMessage?.Invoke(this, msg);

            } else if (_pending.TryRemove(msg.Header.Seq, out var tcs))
            {
                //ResponseMessage
                if (tcs.ResultType is null)
                {
                    tcs.TrySetResult(msg.Payload);
                }
                else
                {
                    var obj = GetSerializer(msg.Header.SerializeType).Deserialize(tcs.ResultType, msg.Payload);
                    if (obj is WithMetadata o)
                        o._metadata = msg.Metadata;
                    tcs.TrySetResult(obj);
                }
            }
        }
        #endregion
    }
}
