using System;
using System.Collections.Generic;

namespace rpcx.net.Client.ServiceDiscovery
{
    public class P2PDiscovery : IServiceDiscovery
    {
        protected string _server;
        protected string _metadata;

        public event WatcherHandler ServiceWatcher;

        public P2PDiscovery(string server, string metadata)
        {
            _server = server;
            _metadata = metadata;
        }

        public IServiceDiscovery Clone(string servicePath) => this;

        public List<KeyValuePair<string, string>> GetServices() =>
            new List<KeyValuePair<string, string>>(1) { new KeyValuePair<string, string> ( _server, _metadata) };

        public void SetFilter(Func<KeyValuePair<string, string>, bool> filter) { }

        public void Close() { }
    }
}
