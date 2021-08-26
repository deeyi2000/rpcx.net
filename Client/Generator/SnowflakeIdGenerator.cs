using System;

namespace rpcx.net.Client.Generator
{
    public class SnowflakeIdGenerator : IIdGenerator<long>
    {
        private static readonly DateTime StartTime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly long twepoch = 687888001020L; //唯一时间随机量
        private static SnowflakeIdGenerator _default;
        public static SnowflakeIdGenerator Default
        {
            get
            {
                if (_default is null)
                    _default = new SnowflakeIdGenerator();
                return _default;
            }
        }

        public long MachineId { get; }
        public long DatacenterId { get; }
        private long _sequence;
        private long _lastTimestamp;

        private SnowflakeIdGenerator(long machineId = 0, long datacenterId = -1)
        {
            MachineId = Math.Min(Math.Max(0, machineId), 0x1F);
            DatacenterId = Math.Min(Math.Max(0, datacenterId), 0x1F);
        }

        public long Next()
        {
            var timestamp = (DateTime.UtcNow - StartTime).Ticks / 10000;
            var sequence = 0L;
            lock (this)
            {
                if (timestamp > _lastTimestamp)
                {
                    _sequence = 0L;
                }
                else
                {
                    _sequence++;
                    if (0 == (_sequence &= 0x0FFF))
                        timestamp++;
                }
                _lastTimestamp = timestamp;
                sequence = _sequence;
            }

            return (((timestamp - twepoch) << 22)
                  | (DatacenterId << 17)
                  | (MachineId << 12)
                  | sequence) & 0x7FFFFFFFFFFFFFFFL;
        }
    }
}
