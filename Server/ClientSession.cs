﻿namespace Server;

public class ClientSession : Session
{
	public JEvent OnClosed { get; set; } = new();
	public bool ParsingPacket { get; set; }

	private int _sendRegistered;
	private JobQueue _sendQueue;
	private JobQueue _parserQueue;
	private ConcurrentQueue<BasePacket> _sendingPacketQueue;
	private IEnumerator<float> _coPacketParserHandler;

	public override void Init(int id, Socket socket)
	{
		base.Init(id, socket);
		_sendingPacketQueue = new();
		_sendRegistered = 0;
		_sendQueue = JobMgr.GetQueue(Define.PacketSendQueueName);
		_parserQueue = JobMgr.GetQueue(Define.PacketParserQueueName);
		_coPacketParserHandler = _recvBuffer.ReadPacket(this);
	}

	public override void OnConnected()
	{
		base.OnConnected();
		LogMgr.Log($"[server] connecting to {_socket.RemoteEndPoint} completed", TraceSourceType.Session, TraceSourceType.Network);
	}

	public override void Close()
	{
		LogMgr.Log($"Session [{Id}] close started", TraceSourceType.Session, TraceSourceType.Console);
		OnClosed.Invoke();
		base.Close();
		LogMgr.Log($"Session [{Id}] close completed", TraceSourceType.Session, TraceSourceType.Console);
	}

	public override bool RegisterSend(BasePacket packet)
	{
		var result = true;
		_sendingPacketQueue.Enqueue(packet);
		if (Interlocked.CompareExchange(ref _sendRegistered, 1, 0) == 0)
		{
			while (_sendingPacketQueue.TryDequeue(out BasePacket item))
			{
				result &= _sendBuffer.WritePacket(item);
			}

			_sendQueue.Push(() => Send());
		}

		return result;
	}

	protected override void Send()
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
		if (_sendingPacketQueue.IsEmpty)
		{
			_sendRegistered = 0;
			return;
		}

		_sendQueue.Push(() =>
		{
			while (_sendingPacketQueue.TryDequeue(out BasePacket item))
			{
				_sendBuffer.WritePacket(item);
			}

			_sendQueue.Push(() => Send());
		});
	}

	protected override void OnRecvCompleted(SocketAsyncEventArgs args)
	{
		base.OnRecvCompleted(args);
		if (args.BytesTransferred == 0)
		{
			SessionMgr.Close(Id);
			return;
		}

		_parserQueue.Push(() =>
		{
			_coPacketParserHandler.MoveNext();

			// todo
			// 만약 한 악성 클라이언트가 엄청 빠르게 많이 보내면 queue가 해당 클라이언트의 패킷만 처리하면서 막힐 수 있다.
			RegisterRecv();
		});
	}
}
