﻿using System;
using System.Net;
using System.Net.Sockets;


namespace ServerCore
{
	public class Connector
	{
		Socket _socket;
		readonly Func<Socket, Session> _sessionFactory;
		public Connector(Func<Socket, Session> func)
		{
			_sessionFactory += func;
		}
		public void StartConnect(IPEndPoint endPoint)
		{
			_socket = new(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			var args = new SocketAsyncEventArgs();
			args.Completed += (obj, args) => OnConnectCompleted(args);
			args.RemoteEndPoint = endPoint;
			if (_socket.ConnectAsync(args) == false)
			{
				OnConnectCompleted(args);
			}
		}
		void OnConnectCompleted(SocketAsyncEventArgs args)
		{
			if (args.SocketError != SocketError.Success) throw new Exception();
			_sessionFactory.Invoke(args.ConnectSocket).OnConnected();
		}
	}
}
