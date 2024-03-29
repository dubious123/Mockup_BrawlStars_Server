﻿namespace Server;

public class ClientSession : Session
{
	public JEvent OnClosed { get; set; } = new();
	public bool ParsingPacket { get; set; }

	private int _sendRegistered;
	private JobQueue _packetQueue;
	private ConcurrentQueue<BasePacket> _sendingPacketQueue;
	private IEnumerator<float> _coPacketParserHandler;

	public override void Init(int id, Socket socket)
	{
		base.Init(id, socket);
		_sendingPacketQueue = new();
		_sendRegistered = 0;
		_packetQueue = JobMgr.GetQueue(Define.PacketSendQueueName);
		_coPacketParserHandler = _recvBuffer.ReadPacket(this);
	}

	public override void OnConnected()
	{
		base.OnConnected();
		Loggers.Network.Information("[server] connecting to {0} completed", _socket.RemoteEndPoint);
	}

	public override void Close()
	{
		Loggers.Network.Information("Session [{Id}] close started", Id);
		OnClosed.Invoke();
		base.Close();
		Loggers.Network.Information("Session [{Id}] close completed", Id);
	}

	//public override bool RegisterSend(BasePacket packet)
	//{
	//	var result = true;
	//	_sendingPacketQueue.Enqueue(packet);
	//	if (Interlocked.CompareExchange(ref _sendRegistered, 1, 0) == 0)
	//	{
	//		while (_sendingPacketQueue.TryDequeue(out BasePacket item))
	//		{
	//			result &= _sendBuffer.WritePacket(item);
	//		}

	//		Send();
	//	}

	//	return result;
	//}

	public override bool RegisterSend(BasePacket packet)
	{
		_sendingPacketQueue.Enqueue(packet);
		if (Interlocked.CompareExchange(ref _sendRegistered, 1, 0) == 0)
		{
			_packetQueue.Push(InternalSendJob);
		}

		return true;
	}

	protected override void Send()
	{
		try
		{
			base.Send();
		}
		catch (ObjectDisposedException e)
		{
			Loggers.Network.Error("Session [{0}] : {1}", Id, e.Message);
			Loggers.Console.Error("Session [{0}] : {1}", Id, e.Message);
		}
		catch (Exception)
		{
			throw;
		}
	}

	protected override void Shutdown()
	{
		Loggers.Network.Information("Session [{Id}] shut down started", Id);
		base.Shutdown();
		Loggers.Network.Information("Session [{Id}] shut down completed", Id);
	}

	protected override void Disconnect()
	{
		Loggers.Network.Information("Session [{Id}] disconnect started", Id);
		base.Disconnect();
		Loggers.Network.Information("Session [{Id}] disconnect completed", Id);
		Loggers.Console.Information("Session [{Id}] disconnect completed", Id);
	}

	//protected override void OnSendCompleted(SocketAsyncEventArgs args)
	//{
	//	base.OnSendCompleted(args);
	//	if (_sendingPacketQueue.IsEmpty)
	//	{
	//		_sendRegistered = 0;
	//		return;
	//	}

	//	while (_sendingPacketQueue.TryDequeue(out BasePacket item))
	//	{
	//		_sendBuffer.WritePacket(item);
	//	}

	//	Send();
	//}

	protected override void OnSendCompleted(SocketAsyncEventArgs args)
	{
		base.OnSendCompleted(args);
		//if (_sendingPacketQueue.IsEmpty)
		//{
		//	_sendRegistered = 0;
		//	return;
		//}

		//while (_sendingPacketQueue.TryDequeue(out BasePacket item))
		//{
		//	_sendBuffer.WritePacket(item);
		//}

		//Send();
	}

	protected override void OnRecvCompleted(SocketAsyncEventArgs args)
	{
		base.OnRecvCompleted(args);
		if (args.BytesTransferred == 0)
		{
			Loggers.Network.Information("Closing[{0}]", Id);
			SessionMgr.Close(Id);
			return;
		}

		_coPacketParserHandler.MoveNext();

		// todo
		// 만약 한 악성 클라이언트가 엄청 빠르게 많이 보내면 queue가 해당 클라이언트의 패킷만 처리하면서 막힐 수 있다.
		RegisterRecv();
	}

	private void InternalSendJob()
	{
		Interlocked.Exchange(ref _sendRegistered, 0);
		while (_sendingPacketQueue.TryDequeue(out BasePacket item))
		{
			_sendBuffer.WritePacket(item);
		}

		Send();
	}
}
