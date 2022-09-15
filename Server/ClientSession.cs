using System;
using System.Diagnostics;
using System.Net.Sockets;
using Server.Game;
using Server.Game.Managers;
using Server.Log;
using Server.Utils;
using ServerCore;
using ServerCore.Managers;
using ServerCore.Packets;
using static Server.Utils.Enums;

namespace Server
{
	public class ClientSession : Session
	{
		public JEvent OnClosed = new();
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
			LogMgr.Log($"[server] connecting to {_socket.RemoteEndPoint} completed", TraceSourceType.Session, TraceSourceType.Network);
		}
		public override void Send()
		{
			try
			{
				base.Send();
			}
			catch (ObjectDisposedException e)
			{
				LogMgr.Log($"Session [{Id}] : {e.Message}", TraceEventType.Error, TraceSourceType.Session);
			}
			catch (Exception)
			{
				throw;
			}
		}
		public override void Close()
		{
			LogMgr.Log($"Session [{Id}] close started", TraceSourceType.Session, TraceSourceType.Console);
			OnClosed.Invoke();
			base.Close();
			LogMgr.Log($"Session [{Id}] close completed", TraceSourceType.Session, TraceSourceType.Console);
		}
		protected override void Shutdown()
		{
			LogMgr.Log($"Session [{Id}] shut down started", TraceSourceType.Session, TraceSourceType.Console);
			base.Shutdown();
			LogMgr.Log($"Session [{Id}] shut down completed", TraceSourceType.Session, TraceSourceType.Console);
		}
		protected override void Disconnect()
		{
			LogMgr.Log($"Session [{Id}] disconnect started", TraceSourceType.Session, TraceSourceType.Console);
			base.Disconnect();
			LogMgr.Log($"Session [{Id}] disconnect completed", TraceSourceType.Session, TraceSourceType.Console);
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
				LogMgr.Log($"From session [{Id}] Read packet {packet}", TraceSourceType.Packet, TraceSourceType.Session);
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
