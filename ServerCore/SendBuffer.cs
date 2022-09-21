using System;
using System.Net.Sockets;
using System.Text.Json;
using System.Buffers;

namespace ServerCore
{
	public class SendBuffer : IBufferWriter<byte>
	{
		readonly byte[] _buffer;
		readonly int _limit;
		int _writePos;
		ArraySegment<byte> _writeSegment => new ArraySegment<byte>(_buffer, _writePos, _limit - _writePos);
		public SendBuffer(ushort size)
		{
			_buffer = new byte[size];
			_writePos = 0;
			_limit = _buffer.Length;
		}
		public ArraySegment<byte> Write(ushort size)
		{
			var segment = new ArraySegment<byte>(_buffer, _writePos, size);
			_writePos += size;
			return segment;
		}
		public void SetBuffer(SocketAsyncEventArgs args)
		{
			args.SetBuffer(_buffer, 0, _writePos);
			_writePos = 0;
		}
		public ArraySegment<byte> Flush()
		{
			var result = new ArraySegment<byte>(_buffer, 0, _writePos);
			_writePos = 0;
			return result;
		}

		public void Advance(int count)
		{
			_writePos += count;
			if (_writePos > _limit)
				throw new Exception();
		}

		public Memory<byte> GetMemory(int sizeHint = 0)
		{
			return _writeSegment;
		}

		public Span<byte> GetSpan(int sizeHint = 0)
		{
			return _writeSegment;
		}
	}
}
