using System.Collections;
using System.Collections.Generic;

namespace rpcx.net.Client {

    // 增加 IEnumerable 接口实现, 方便调用

    public abstract class WithMetadata : IEnumerable<KeyValuePair<string, string>> {
        internal protected Dictionary<string, string> _metadata;
        public virtual string this[string k] {
            get {
                if (_metadata != null && _metadata.TryGetValue(k, out var v))
                    return v;
                return null;
            }
            set {
                if (_metadata is null)
                    _metadata = new Dictionary<string, string>(10);
                _metadata[k] = value;
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() {
            return _metadata.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _metadata.GetEnumerator();
        }
    }
}
