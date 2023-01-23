using System;
using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
	public class Connector
	{
		private Socket _socket;
		private readonly Func<Socket, Session> _sessionFactory;
		public Connector(Func<Socket, Session> func)
		{
			_sessionFactory += func;
		}

		public void StartConnect(IPEndPoint endPoint)
		{
			_socket = new(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			_socket.NoDelay = true;
			var args = new SocketAsyncEventArgs();
			args.Completed += (obj, args) => OnConnectCompleted(args);
			args.RemoteEndPoint = endPoint;
			if (_socket.ConnectAsync(args) == false)
			{
				OnConnectCompleted(args);
			}
		}

		private void OnConnectCompleted(SocketAsyncEventArgs args)
		{
			if (args.SocketError != SocketError.Success) throw new Exception();
			_sessionFactory.Invoke(args.ConnectSocket).OnConnected();
		}
	}
}
