using System.Collections.Generic;

namespace rpcx.net.Client
{
    public abstract class WithMetadata
    {
        internal protected Dictionary<string, string> _metadata;
        public virtual string this[string k]
        {
            get
            {
                if (_metadata != null && _metadata.TryGetValue(k, out var v))
                    return v;
                return null;
            }
            set
            {
                if (_metadata is null)
                    _metadata = new Dictionary<string, string>(10);
                _metadata[k] = value;
            }
        }
    }
}
