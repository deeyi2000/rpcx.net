using System;
using System.Collections.Generic;

namespace rpcx.net.Shared {
    public interface IContext {
        object Key();
        object Value(object key);
        void SetValue(object key, object val);

        IContext Parent();
        IContext FindParent(object key);
    }

    public sealed class Context {
        /// <summary>
        /// ReqMetaDataKey is used to set metatdata in context of requests.
        /// refrence: rpcx -> ReqMetaDataKey
        /// </summary>
        public const string ReqMetaDataKey = "__req_metadata";

        /// <summary>
        /// ResMetaDataKey is used to set metatdata in context of responses.
        /// refrence: rpcx -> ResMetaDataKey
        /// </summary>
        public const string ResMetaDataKey = "__res_metadata";

        public static IContext WithValue(IContext parent, object key, object val = null) {
            return new valueCtx {
                _parent = parent,
                _key = key,
                _val = val
            };
        }

        /// <summary>
        /// 设置 medadata
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="val">Medatada</param>
        /// <returns></returns>
        public static IContext WithMetadata(IContext parent, IDictionary<string, string> val) {
            return WithValue(parent, ReqMetaDataKey, val);
        }
    }

    class valueCtx : IContext {
        internal IContext _parent;

        internal object _key;
        internal object _val;

        public object Key() => _key;

        public object Value(object key) {
            if (_key == key) {
                return _val;
            }
            if (_parent == null) {
                return null;
            }
            return _parent.Value(key);
        }

        public void SetValue(object key, object val) {
            var ctx = FindParent(key) as valueCtx;
            if (ctx == null) {
                throw new Exception($"当前对象上下文中，无法找到 [Key = {key} ] 对应的节点。");
            }
            ctx._val = val;
        }

        public IContext Parent() => _parent;

        public IContext FindParent(object key) {
            if (this._key == key) {
                return this;
            }

            var parent = this._parent;
            if (parent == null) {
                this._parent = Context.WithValue(null, key);
                return this._parent;
            }
            return parent.FindParent(key);
        }
    }
}
