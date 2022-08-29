using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
	public class ClientSession : Session
	{
		public override void OnConnected()
		{
			base.OnConnected();
			Console.WriteLine($"[server] connecting to {_socket.RemoteEndPoint} completed");
		}
		protected override void OnSendCompleted(SocketAsyncEventArgs args)
		{
			base.OnSendCompleted(args);
		}
		protected override void OnRecvCompleted(SocketAsyncEventArgs args)
		{
			base.OnRecvCompleted(args);
			PacketHandler.HandlePackets(_recvBuffer.ReadAll());
			RegisterRecv();
		}


	}
}
