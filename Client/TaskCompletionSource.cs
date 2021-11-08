using rpcx.net.Shared.Protocol;
using System;
using System.Threading.Tasks;

namespace rpcx.net.Client {
    public class TaskCompletionSource : TaskCompletionSource<object> {
        public Type ResultType { get; }

        // 增加原始参数数据
        public Message Message { get; }

        public TaskCompletionSource(Type resultType, Message message) : this() {
            Message = message;
            ResultType = resultType;
        }

        public TaskCompletionSource() : base() { }

    }
}
