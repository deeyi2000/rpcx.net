using System;
using System.Collections.Generic;
using System.Text;

namespace rpcx.net.Client.ServiceDiscovery
{
    public class ZookeeperDiscovery : IServiceDiscovery
    {
        public event WatcherHandler ServiceWatcher;

        public IServiceDiscovery Clone(string servicePath)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public List<KeyValuePair<string, string>> GetServices()
        {
            throw new NotImplementedException();
        }

        public void SetFilter(Func<KeyValuePair<string, string>, bool> filter)
        {
            throw new NotImplementedException();
        }
    }
}
