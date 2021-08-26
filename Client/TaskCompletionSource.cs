using System;
using System.Threading.Tasks;

namespace rpcx.net.Client
{
    public class TaskCompletionSource : TaskCompletionSource<object>
    {
        public Type ResultType { get; }

        public TaskCompletionSource(Type resultType) : this()
        {
            ResultType = resultType;
        }

        public TaskCompletionSource() : base() { }

    }
}
