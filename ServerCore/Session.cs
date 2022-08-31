using ServerCore.Packets;
using System;
using System.Net.Sockets;

namespace ServerCore
{
	public abstract class Session
	{
		public int Id;
		protected Socket _socket;
		protected SendBuffer _sendBuffer = new(4048);
		protected RecvBuffer _recvBuffer = new(32768);
		protected SocketAsyncEventArgs _sendArgs = new();
		protected SocketAsyncEventArgs _recvArgs = new();
		public void Init(int id, Socket socket)
		{
			Id = id;
			_socket = socket;
			_sendArgs.Completed += (obj, e) => OnSendCompleted(e);
			_recvArgs.Completed += (obj, e) => OnRecvCompleted(e);
		}
		public virtual void OnConnected()
		{
			RegisterRecv();
		}
		public abstract bool RegisterSend(BasePacket packet);

		public void Send()
		{
			_sendArgs.SetBuffer(_sendBuffer.Flush());
			if (_socket.SendAsync(_sendArgs) == false)
				OnSendCompleted(_sendArgs);
		}
		public virtual void Close()
		{
			Shutdown();
			Disconnect();
			_socket.Close(100);
		}
		protected virtual void OnSendCompleted(SocketAsyncEventArgs args)
		{
			if (args.SocketError != SocketError.Success)
				throw new Exception();
		}


		protected virtual void RegisterRecv()
		{
			_recvBuffer.Clear();
			_recvArgs.SetBuffer(_recvBuffer.GetWriteBuffer());
			if (_socket.ReceiveAsync(_recvArgs) == false)
			{
				OnRecvCompleted(_recvArgs);
			}
		}
		protected virtual void OnRecvCompleted(SocketAsyncEventArgs args)
		{
			if (args.SocketError != SocketError.Success)
				throw new Exception();
			_recvBuffer.OnWrite(args.BytesTransferred);
		}
		protected virtual void Disconnect()
		{
			_socket.Disconnect(false);
		}
		protected virtual void Shutdown()
		{
			_socket.Shutdown(SocketShutdown.Both);
		}






	}
}
