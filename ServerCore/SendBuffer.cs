using System;
using Google.Protobuf;
using static ServerCore.Utils.Enums;

namespace ServerCore
{
	public class SendBuffer
	{
		readonly byte[] _buffer;
		readonly int _limit;
		int _writePos;
		public SendBuffer(ushort size)
		{
			_buffer = new byte[size];
			_writePos = 0;
			_limit = _buffer.Length;
		}
		public bool Write(IMessage msg, PacketId id)
		{
			var size = (ushort)msg.CalculateSize();
			if (_limit - _writePos - 4 - size < 0) return false;
			Array.Copy(BitConverter.GetBytes((ushort)id), 0, _buffer, _writePos, 2);
			Array.Copy(BitConverter.GetBytes(size), 0, _buffer, _writePos + 2, 2);
			msg.WriteTo(new ArraySegment<byte>(_buffer, _writePos, size));
			_writePos += size + 4;
			return true;
		}
		public ArraySegment<byte> Flush()
		{
			var result = new ArraySegment<byte>(_buffer, 0, _writePos);
			_writePos = 0;
			return result;
		}
	}
}
