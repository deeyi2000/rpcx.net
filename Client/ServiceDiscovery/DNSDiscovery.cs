using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Timers;

namespace rpcx.net.Client.ServiceDiscovery
{
    public class DNSDiscovery : IServiceDiscovery
    {
        protected string _domain;
        protected string _network;
        protected int _port;
        protected double _duration;
        protected Timer _watchTimer;
        protected Func<KeyValuePair<string, string>, bool> _filter;
        protected Dictionary<string, string> _services;

        public event WatcherHandler ServiceWatcher;

        public DNSDiscovery(string domain, string network, int port, double duration)
        {
            _domain = domain;
            _network = network;
            _port = port;
            _duration = duration;

            Watch(this, null);
        }

        private void Watch(object sender, ElapsedEventArgs e)
        {
            if(_watchTimer is null)
            {
                _watchTimer = new Timer()
                {
                    AutoReset = false,
                };
                _watchTimer.Elapsed += Watch;
            }
            Lookup();
            _watchTimer.Interval = _duration;
            _watchTimer.Start();
        }

        public IServiceDiscovery Clone(string servicePath) => new DNSDiscovery(_domain, _network, _port, _duration);

        public Dictionary<string, string> GetServices()
        {
            lock (_services)
                return _services;
        }

        public void SetFilter(Func<KeyValuePair<string, string>, bool> filter) => _filter = filter;


        public void Close()
        {
            _watchTimer.Stop();
        }

        protected void Lookup()
        {
            var ips = Dns.GetHostAddresses(_domain);
            var svcs = ips.Select(ip => new KeyValuePair<string, string>($"{_network}@{ip}:{_port}", null))
                          .Where(_filter)
                          .OrderBy(kv => kv.Key)
                          .ToDictionary(kv => kv.Key, kv => kv.Value);

            lock (_services)
                _services = svcs;
            lock (ServiceWatcher)
                ServiceWatcher?.Invoke(this, svcs);
        }
    }
}
