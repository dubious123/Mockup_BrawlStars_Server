using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ServerCore;
using ServerCore.Managers;
using ServerCore.Packets;

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
			if (args.BytesTransferred == 0)
			{
				SessionMgr.Close(Id);
				return;
			}
			while (_recvBuffer.CanRead())
			{
				PacketHandler.HandlePacket(_recvBuffer.ReadPacket(), this);

			}
			RegisterRecv();
		}

		public override bool RegisterSend(BasePacket packet)
		{
			return _sendBuffer.WritePacket(packet);
		}
	}
}
