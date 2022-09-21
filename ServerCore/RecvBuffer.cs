using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
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
		public bool CanRead()
		{
			return _writePos - _readPos > 4;
		}

		public bool CanRead(ushort size)
		{
			return _writePos - _readPos - size >= 0;

		}

		public ArraySegment<byte> Read(ushort size)
		{
			//Console.WriteLine($"[reading] readPos : {_readPos}, writePos : {_writePos}, size : {size}");
			var result = new ArraySegment<byte>(_buffer, _readPos, size);
			Interlocked.Add(ref _readPos, size);
			Debug.Assert(_readPos <= _writePos);
			return result;
		}
		public ArraySegment<byte> Read(int count)
		{
			//Console.WriteLine($"[reading] readPos : {_readPos}, writePos : {_writePos}, size : {count}");
			var segment = new ArraySegment<byte>(_buffer, _readPos, count);
			Interlocked.Add(ref _readPos, count);
			Debug.Assert(_readPos <= _writePos);
			return segment;
		}
		public ArraySegment<byte> GetWriteBuffer()
		{
			return new ArraySegment<byte>(_buffer, _writePos, _buffer.Length - _writePos);
		}
		public void OnWrite(int size)
		{
			Debug.Assert(_writePos + size < _buffer.Length);
			//Console.WriteLine($"on write, readPos : {_readPos}, writePos : {_writePos}, size : {size}");
			Interlocked.Add(ref _writePos, size);
		}
		/// <summary>
		/// [][]...[R]...[W]...[][]
		/// [R]...[W]..........[][]
		/// </summary>
		public void Clear()
		{
			//Console.WriteLine($"clearing, readPos : {_readPos}, writePos : {_writePos}, size : {_writePos - _readPos}");
			var leftDataSize = _writePos - _readPos;
			if (leftDataSize == 0)
			{
				_writePos = _readPos = 0;
				return;
			}
			if (_readPos == 0)
			{
				return;
			}
			Array.Copy(_buffer, _readPos, _buffer, 0, leftDataSize);
			_writePos = leftDataSize;
			_readPos = 0;
			return;
		}
	}
}
