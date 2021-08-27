using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Timers;
using Zeroconf;
using static rpcx.net.Shared.Utils;

namespace rpcx.net.Client.ServiceDiscovery
{
    public class mDNSDiscovery : IServiceDiscovery
    {
        protected string _domain;
        protected string _service;
        protected double _timeout;
        protected double _watchInterval;
        protected Timer _watchTimer;
        protected Func<KeyValuePair<string, string>, bool> _filter;
        protected List<KeyValuePair<string, string>> _services;

        public event WatcherHandler ServiceWatcher;

        protected struct Svc
        {
            public string service { get; set; }

            public string service_address { get; set; }
        }

        public mDNSDiscovery(string service, double timeout, double watchInterval, string domain = "local.")
        {
            _domain = domain;
            _service = service;
            _timeout = timeout;
            _watchInterval = watchInterval;

            Watch(this, null);
        }

        private void Watch(object sender, ElapsedEventArgs e)
        {
            if (_watchTimer is null)
            {
                _watchTimer = new Timer()
                {
                    AutoReset = false,
                };
                _watchTimer.Elapsed += Watch;
            }
            Browse();
            _watchTimer.Interval = _watchInterval;
            _watchTimer.Start();
        }

        public IServiceDiscovery Clone(string servicePath) => new mDNSDiscovery(_service, _timeout, _watchInterval, _domain);

        public void Close()
        {
            _watchTimer.Stop();
        }

        public List<KeyValuePair<string, string>> GetServices() => _services;

        public void SetFilter(Func<KeyValuePair<string, string>, bool> filter) => _filter = filter;

        protected async void Browse()
        {
            var ips = await ZeroconfResolver.ResolveAsync($"_rpcxservices.{_domain}");
            var ss = ips.SelectMany(ip =>
               {
                   var k = ip.Services.Values.First().Properties.First().Keys.First();
                   return JsonSerializer.Deserialize<List<Svc>>(k.UrlDecode());
               }).Select(svc => new KeyValuePair<string, string>(svc.service_address, svc.service));
            ss = _filter is null ? ss : ss.Where(_filter);
            var svcs = ss.ToList();

            _services = svcs;
            ServiceWatcher?.Invoke(this, svcs);
        }
    }
}
