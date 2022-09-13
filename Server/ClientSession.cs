using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Server.Utils;
using ServerCore;
using ServerCore.Managers;
using ServerCore.Packets;

namespace Server
{
	public class ClientSession : Session
	{
		JobQueue _sendQueue;
		JobQueue _handlerQueue;
		bool _sendRegistered;
		public override void Init(int id, Socket socket)
		{
			base.Init(id, socket);
			_sendRegistered = false;
			_sendQueue = JobMgr.GetQueue(Define.PacketSendQueueName);
			_handlerQueue = JobMgr.GetQueue(Define.PacketHandlerQueueName);
		}
		public override void OnConnected()
		{
			base.OnConnected();
			Console.WriteLine($"[server] connecting to {_socket.RemoteEndPoint} completed");
		}
		protected override void OnSendCompleted(SocketAsyncEventArgs args)
		{
			base.OnSendCompleted(args);
			_sendRegistered = false;
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
				var packet = _recvBuffer.ReadPacket();
				_handlerQueue.Push(() =>
				 PacketHandler.HandlePacket(packet, this));
			}
			RegisterRecv();
		}

		public override bool RegisterSend(BasePacket packet)
		{
			var result = _sendBuffer.WritePacket(packet);
			if (_sendRegistered == false)
			{
				_sendQueue.Push(() => Send());
				_sendRegistered = true;
			}
			return result;
		}
	}
}
