using Consul;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace rpcx.net.Client.ServiceDiscovery
{
    public class ConsulDiscovery : IServiceDiscovery
    {
        protected string _basePath;
        protected IConsulClient _client;
        protected CancellationTokenSource _cts;
        protected Func<KeyValuePair<string, string>, bool> _filter;
        protected List<KeyValuePair<string, string>> _services;

        public event WatcherHandler ServiceWatcher;

        public ConsulDiscovery(string basePath, string servicePath, ConsulClientConfiguration config) :
            this($"{basePath}/{servicePath}", new ConsulClient(config)) { }

        public ConsulDiscovery(string basePath, IConsulClient client)
        {
            _basePath = basePath.Trim('/');
            _client = client;
            _cts = new CancellationTokenSource();

            Watch();
        }

        private void Watch()
        {
            var token = _cts.Token;
            var opt = new QueryOptions()
            {
                WaitTime = TimeSpan.FromSeconds(15)
            };
            var prefix = $"{_basePath}/";
            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    var resp = await _client.KV.List(_basePath, opt, token);
                    if (resp is null || resp.LastIndex == opt.WaitIndex)
                        continue;
                    opt.WaitIndex = resp.LastIndex;

                    var ss = resp.Response.Where(kv => kv.Key.StartsWith(prefix))
                                            .Select(kv => new KeyValuePair<string, string>(kv.Key.Substring(prefix.Length), Encoding.UTF8.GetString(kv.Value)));
                    ss = _filter is null ? ss : ss.Where(_filter);
                    var svcs = ss.ToList();

                    Interlocked.Exchange(ref _services, svcs);
                    ServiceWatcher?.Invoke(this, svcs);
                }
            }, token);
        }

        public IServiceDiscovery Clone(string servicePath) => new ConsulDiscovery($"{_basePath}/{servicePath}", _client);
        public List<KeyValuePair<string, string>> GetServices() => _services;
        public void SetFilter(Func<KeyValuePair<string, string>, bool> filter) => Interlocked.Exchange(ref _filter, filter);
        public void Close() => _cts.Cancel();
    }
}
