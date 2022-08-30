using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ServerCore;
using ServerCore.Packets.Client;
using static ServerCore.Utils.Enums;

namespace TestClient
{
	public class ServerSession : Session
	{
		public override void OnConnected()
		{
			base.OnConnected();
			Console.WriteLine($"[client] connecting to {_socket.RemoteEndPoint} completed");
			C_Chat packet = new C_Chat
			{
				Id = (ushort)PacketId.C_Chat
			};
			RegisterSend(packet);
			Send();
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
