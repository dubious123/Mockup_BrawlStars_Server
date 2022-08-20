using System;
using System.Threading;
using System.Net.Sockets;
using static ServerCore.Utils.Tools;


namespace TestClient
{
	class Program
	{
		static void Main(string[] args)
		{
			var endPoint = GetNewEndPoint(7777);
			while (true)
			{
				var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				var e = new SocketAsyncEventArgs();
				e.RemoteEndPoint = endPoint;
				e.Completed += (obj, args) =>
				{
					var a = new SocketAsyncEventArgs();
					a.SetBuffer(new byte[] { 1, 1, 1, 1 });
					args.ConnectSocket.SendAsync(a);
				};
				if (socket.ConnectAsync(e) == false)
				{
					var a = new SocketAsyncEventArgs();
					a.SetBuffer(new byte[] { 1, 1, 1, 1 });
					e.ConnectSocket.SendAsync(a);
				}
				Thread.Sleep(1000);

			}
		}
	}
}
