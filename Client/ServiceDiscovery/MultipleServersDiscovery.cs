using System;
using System.Collections.Generic;

namespace rpcx.net.Client.ServiceDiscovery
{
    public class MultipleServersDiscovery : IServiceDiscovery
    {
        protected List<KeyValuePair<string, string>> _services;

        public event WatcherHandler ServiceWatcher;

        public MultipleServersDiscovery(List<KeyValuePair<string, string>> services)
        {
            _services = services;
        }

        public void Update(List<KeyValuePair<string, string>> services)
        {
            lock (_services)
                _services = services;
            lock (ServiceWatcher)
                ServiceWatcher?.Invoke(this, services);
        }

        public IServiceDiscovery Clone(string servicePath) => this;

        public void Close() { }

        public List<KeyValuePair<string, string>> GetServices() => _services;

        public void SetFilter(Func<KeyValuePair<string, string>, bool> filter) { }
    }
}
