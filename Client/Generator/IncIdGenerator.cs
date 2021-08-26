using System.Threading;

namespace rpcx.net.Client.Generator
{
    public class IncIdGenerator : IIdGenerator<long>
    {
        private static IncIdGenerator _default;
        public static IncIdGenerator Default
        {
            get
            {
                if (_default is null)
                    _default = new IncIdGenerator();
                return _default;
            }
        }

        protected long _count;

        public IncIdGenerator(long start = 0)
        {
            _count = start;
        }

        public long Next() => Interlocked.Increment(ref _count);
    }
}
