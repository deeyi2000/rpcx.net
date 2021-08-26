using System;
using System.Runtime.InteropServices;

namespace rpcx.net.Shared.Protocol
{
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
	public class Header
	{
		public enum eTypeMask : ushort
		{
			messageType = 0x8000,
			heartBeat = 0x4000,
			oneWay = 0x2000,
			compressType = 0x1C00,
			messageStatusType = 0x0300,
			serializeType = 0x00F0,
		}

		public enum eType : ushort
		{
			None =       0,
			//messageType
			Request =    0,
			Response =   0x8000,

			HeartBeat =  0x4000,
			OneWay =     0x2000,

			//compressType
			NoCompress = 0x0000,
			Gzip =       0x0400,
			Snappy =     0x0800,

			//messageStatusType
			Normal = 0x0000,
			Error = 0x0100,

			//serializeType
			Byte = 0x0000,
			JSON = 0x0010,
			Protobuf = 0x0020,
			MessagePack = 0x0030,
		}

		private eType GetType(eTypeMask mask) => (eType)((ushort)types & (ushort)mask);
		private void SetType(eType val, eTypeMask mask) => types = (eType)(((ushort)types & ~(ushort)mask) | ((ushort)val & (ushort)mask));

		[FieldOffset(0)] private byte magicNumber = 0x08;
		[FieldOffset(1)] private byte version = 0x00;
		[FieldOffset(2)] private eType types = 0x00;
		/*[FieldOffset(2)] private eType messageType;
		[FieldOffset(2)] private eType heartbeat;
		[FieldOffset(2)] private eType oneWay;
		[FieldOffset(2)] private eType compressType;
		[FieldOffset(2)] private eType messageStatusType;
		[FieldOffset(3)] private eType serializeType;
		[FieldOffset(3)] private eType reserve;*/
		[FieldOffset(4)] private long seq;

		public byte MagicNumber { get => magicNumber; }
		public byte Version { get => version; set => version = value; }
		public eType MessageType { get => GetType(eTypeMask.messageType); set => SetType(value, eTypeMask.messageStatusType); }
		public bool IsHeartbeat { get => (types & eType.HeartBeat) == eType.HeartBeat; set => SetType((value ? eType.HeartBeat : eType.None), eTypeMask.heartBeat); }
		public bool IsOneWay { get => (types & eType.OneWay) == eType.OneWay; set => SetType((value ? eType.OneWay : eType.None), eTypeMask.oneWay); }
		public eType CompressType { get => GetType(eTypeMask.compressType); set => SetType(value, eTypeMask.compressType); }
		public eType MessageStatusType { get => GetType(eTypeMask.messageStatusType); set => SetType(value, eTypeMask.messageStatusType); }
		public eType SerializeType { get => GetType(eTypeMask.serializeType); set => SetType(value, eTypeMask.serializeType); }
		public long Seq { get => seq; set => seq = value; }

		public static Header NewRequest(eType types = eType.None, long seq = 0)
		{
			return new Header()
			{
				types = eType.Request | types,
				Seq = seq,
			};
		}

		public static Header NewResponse(eType types = eType.None, long seq = 0)
		{
			return new Header()
			{
				types = eType.Response,
				Seq = seq,
			};
		}

		public byte[] GetBytes()
        {
			return new byte[]
			{
				magicNumber,
				version,
				(byte)((ushort)types >> 8),
				(byte)types,
				(byte)(seq >> 56),
				(byte)(seq >> 48),
				(byte)(seq >> 40),
				(byte)(seq >> 32),
				(byte)(seq >> 24),
				(byte)(seq >> 16),
				(byte)(seq >> 8),
				(byte)seq,
			};
        }

		public static Header FromBytes(byte[] bytes)
        {
			return new Header()
			{
				magicNumber = bytes[0],
				version = bytes[1],
				types = (eType)(bytes[2] << 8 | bytes[3]),
				seq = ((long)bytes[4] << 56) | ((long)bytes[5] << 48) | ((long)bytes[6] << 40) | ((long)bytes[7] << 32) | ((long)bytes[8] << 24) | ((long)bytes[9] << 16) | ((long)bytes[10] << 8) | bytes[11]
            };
        }
	}
}
