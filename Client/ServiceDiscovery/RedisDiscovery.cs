using System;
using System.Collections.Generic;
using System.Text;

namespace rpcx.net.Client.ServiceDiscovery
{
    public class RedisDiscovery : IServiceDiscovery
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

        public Dictionary<string, string> GetServices()
        {
            throw new NotImplementedException();
        }

        public void SetFilter(Func<KeyValuePair<string, string>, bool> filter)
        {
            throw new NotImplementedException();
        }
    }
}
