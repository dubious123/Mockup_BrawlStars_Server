using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
	public class Listener
	{
		Socket _socket;
		public void StartListen(IPEndPoint endPoint)
		{
			_socket = new(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			_socket.Bind(endPoint);
			_socket.Listen(100);
			for (int i = 0; i < 10; i++)
			{
				var args = new SocketAsyncEventArgs();
				args.Completed += (obj, args) => OnAcceptCompleted(args);
				RegisterAccept(args);
			}
		}

		void RegisterAccept(SocketAsyncEventArgs args)
		{
			args.AcceptSocket = null;

			if (_socket.AcceptAsync(args) == false)
			{
				OnAcceptCompleted(args);
			}
		}
		void OnAcceptCompleted(SocketAsyncEventArgs args)
		{
			if (args.SocketError != SocketError.Success) throw new Exception();
			Console.WriteLine("hi");
			RegisterAccept(args);
		}
	}
}
