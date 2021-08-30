using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Timers;
using System.Threading;

namespace rpcx.net.Client.ServiceDiscovery
{
    public class DNSDiscovery : IServiceDiscovery
    {
        protected string _domain;
        protected string _network;
        protected int _port;
        protected double _duration;
        protected System.Timers.Timer _watchTimer;
        protected Func<KeyValuePair<string, string>, bool> _filter;
        protected List<KeyValuePair<string, string>> _services;

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
                _watchTimer = new System.Timers.Timer()
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

        public List<KeyValuePair<string, string>> GetServices() => _services;

        public void SetFilter(Func<KeyValuePair<string, string>, bool> filter) => Interlocked.Exchange(ref _filter, filter);


        public void Close()
        {
            _watchTimer.Stop();
        }

        protected void Lookup()
        {
            var ips = Dns.GetHostAddresses(_domain);
            var ss = ips.Select(ip => new KeyValuePair<string, string>($"{_network}@{ip}:{_port}", null));
            ss = _filter is null ? ss : ss.Where(_filter);
            var svcs = ss.OrderBy(kv => kv.Key)
                         .ToList();

            Interlocked.Exchange(ref _services, svcs);
            ServiceWatcher?.Invoke(this, svcs);
        }
    }
}
