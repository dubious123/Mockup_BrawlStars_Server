using System;
using System.Collections.Generic;
using ServerCore.Managers;
using ServerCore.Packets;

namespace ServerCore
{
	//packet을 handling 하는 도중, 즉 recv을 등록하지 않았는데 client에서 packet을 보내면 어떻게 되는거지?
	public class RecvBuffer
	{
		readonly byte[] _buffer;
		readonly object _lock;
		int _readPos;
		int _writePos;
		public RecvBuffer(ushort size)
		{
			_buffer = new byte[size];
			_readPos = 0;
			_writePos = 0;
			_lock = new object();
		}
		public bool CanRead() => _writePos - _readPos > 4;
		public bool CanRead(ushort size) => _writePos - _readPos - size > 0;
		public ArraySegment<byte> Read(ushort size)
		{
			var result = new ArraySegment<byte>(_buffer, _readPos, size);
			_readPos += size;
			return result;
		}
		public ArraySegment<byte> Read(int count)
		{
			var segment = new ArraySegment<byte>(_buffer, _readPos, count);
			_readPos += count;
			return segment;
		}
		public ArraySegment<byte> GetWriteBuffer() => new ArraySegment<byte>(_buffer, _writePos, _buffer.Length - _writePos);
		public void OnWrite(int size) => _writePos += size;
		public void Clear()
		{
			lock (_lock)
			{
				var leftDataSize = _writePos - _readPos;
				if (leftDataSize != 0 && _readPos != 0)
				{
					Array.Copy(_buffer, _readPos, _buffer, 0, leftDataSize);
					_writePos = leftDataSize;
					_readPos = 0;
					return;
				}
				_readPos = _writePos = 0;
			}
		}
	}
}
