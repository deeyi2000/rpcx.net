using System;
using System.Collections.Generic;

namespace rpcx.net.Client.ServiceDiscovery
{
	//public delegate bool ServiceDiscoveryFilter(KeyValuePair<string, string> service);

	public delegate void WatcherHandler(object sender, Dictionary<string, string> services);

	public interface IServiceDiscovery
	{
		event WatcherHandler ServiceWatcher;
		Dictionary<string, string> GetServices();
		void SetFilter(Func<KeyValuePair<string, string>, bool> filter);
		IServiceDiscovery Clone(string servicePath);
		void Close();
	}
}
