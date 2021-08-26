using System;
using System.Collections.Generic;

namespace rpcx.net.Client.ServiceDiscovery
{
    public class MultipleServersDiscovery : IServiceDiscovery
    {
        protected Dictionary<string, string> _services;

        public event WatcherHandler ServiceWatcher;

        public MultipleServersDiscovery(Dictionary<string, string> services)
        {
            _services = services;
        }

        public void Update(Dictionary<string, string> services)
        {
            lock (_services)
                _services = services;
            lock (ServiceWatcher)
                ServiceWatcher?.Invoke(this, services);
        }

        public IServiceDiscovery Clone(string servicePath) => this;

        public void Close() { }

        public Dictionary<string, string> GetServices() => _services;

        public void SetFilter(Func<KeyValuePair<string, string>, bool> filter) { }
    }
}
