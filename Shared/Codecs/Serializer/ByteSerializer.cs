using System;
using System.Runtime.InteropServices;

namespace rpcx.net.Shared.Codecs.Serializer
{
    public class ByteSerializer : ISerializer
    {
        private static ByteSerializer _default;
        public static ByteSerializer Default { get
            {
                if (_default is null)
                    _default = new ByteSerializer();
                return _default;
            } 
        }

        public object Deserialize(Type type, ReadOnlyMemory<byte> bytes)
        {
            var obj = Activator.CreateInstance(type);

            GCHandle objHandle = GCHandle.Alloc(obj);
            Marshal.Copy(bytes.ToArray(), 0, GCHandle.ToIntPtr(objHandle), Marshal.SizeOf(obj));
            objHandle.Free();
            return obj;
        }

        public byte[] Serialize(object value)
        {
            var buf = new byte[Marshal.SizeOf(value)];
            GCHandle valHandle = GCHandle.Alloc(value);
            Marshal.Copy(GCHandle.ToIntPtr(valHandle), buf, 0, buf.Length);
            valHandle.Free();
            return buf;
        }
    }
}
