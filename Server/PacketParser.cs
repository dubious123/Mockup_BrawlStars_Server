namespace Server
{
	public static class PacketParser
	{
		static readonly ConcurrentDictionary<ushort, Func<ArraySegment<byte>, BasePacket>> _readDict;
		static readonly JsonSerializerOptions _options;
		static JobQueue _packetHandlerQueue;
		static PacketParser()
		{
			_packetHandlerQueue = JobMgr.GetQueue(Define.PacketHandlerQueueName);
#if DEBUG
			_options = new JsonSerializerOptions
			{
				IncludeFields = true,
				WriteIndented = true,
				Converters =
				{
					new Vector2Converter(),
				}
			};
#else
			_options = new JsonSerializerOptions
			{
				IncludeFields = true,
				Converters =
				{
					new Vector2Converter(),
				}
			};
#endif
			_readDict = new ConcurrentDictionary<ushort, Func<ArraySegment<byte>, BasePacket>>();
			_readDict.TryAdd((ushort)PacketId.C_Init, arr => JsonSerializer.Deserialize<C_Init>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.C_Login, arr => JsonSerializer.Deserialize<C_Login>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.C_EnterLobby, arr => JsonSerializer.Deserialize<C_EnterLobby>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.C_EnterGame, arr => JsonSerializer.Deserialize<C_EnterGame>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.C_GameReady, arr => JsonSerializer.Deserialize<C_GameReady>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.C_BroadcastPlayerInput, arr => JsonSerializer.Deserialize<C_BroadcastPlayerInput>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.S_Init, arr => JsonSerializer.Deserialize<S_Init>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.S_Login, arr => JsonSerializer.Deserialize<S_Login>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.S_EnterLobby, arr => JsonSerializer.Deserialize<S_EnterLobby>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.S_EnterGame, arr => JsonSerializer.Deserialize<S_EnterGame>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.S_BroadcastEnterGame, arr => JsonSerializer.Deserialize<S_BroadcastEnterGame>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.S_BroadcastStartGame, arr => JsonSerializer.Deserialize<S_BroadcastStartGame>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.S_BroadcastGameState, arr => JsonSerializer.Deserialize<S_BroadcastGameState>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.S_BroadcastMove, arr => JsonSerializer.Deserialize<S_BroadcastMove>(arr, _options));
		}
		//		public static BasePacket ReadPacket(this RecvBuffer buffer)
		//		{
		//			try
		//			{
		//				var id = BitConverter.ToUInt16(buffer.Read(2));
		//				var size = BitConverter.ToUInt16(buffer.Read(2));
		//				_readDict.TryGetValue(id, out Func<ArraySegment<byte>, BasePacket> func);
		//#if DEBUG
		//				var arr = buffer.Read(size);
		//				string json = Encoding.UTF8.GetString(arr);
		//				LogMgr.Log($"size : {size}" + json, TraceSourceType.PacketRecv);
		//				var packet = func.Invoke(arr);
		//				LogMgr.Log($"received packet {JsonSerializer.Serialize(packet, packet.GetType(), _options)}", TraceSourceType.PacketRecv);
		//				return packet;
		//#else
		//				return func.Invoke(buffer.Read(size));
		//#endif
		//			}
		//			catch (System.Exception ex)
		//			{
		//				Console.WriteLine(ex);
		//				throw new Exception();
		//			}
		//		}
		public static IEnumerator<float> ReadPacket(this RecvBuffer buffer, ClientSession session)
		{
			while (true)
			{
				while (buffer.CanRead() == false)
				{
					yield return 0f;
				}
				//string str = string.Empty;
				//var a = buffer.Read(2);

				//str += BitConverter.ToString(a.Array, a.Offset, a.Count);

				//ushort id = BitConverter.ToUInt16(a);
				ushort id = BitConverter.ToUInt16(buffer.Read(2));


				//a = buffer.Read(2);

				//str += BitConverter.ToString(a.Array, a.Offset, a.Count);

				//ushort size = BitConverter.ToUInt16(a);
				ushort size = BitConverter.ToUInt16(buffer.Read(2));
				//Console.WriteLine($"Packet size : {size}");
				//Debug.Assert(size < 200);

				while (buffer.CanRead(size) == false)
				{
					yield return 0f;
				}
				//Todo handle invalid id
				_readDict.TryGetValue(id, out Func<ArraySegment<byte>, BasePacket> func);
#if DEBUG
				var arr = buffer.Read(size);

				//str += BitConverter.ToString(arr.Array, arr.Offset, arr.Count);

				//LogMgr.Log(str, TraceSourceType.Network);

				string json = Encoding.UTF8.GetString(arr);
				//LogMgr.Log($"size : {size}" + json, TraceSourceType.PacketRecv);
				var packet = func.Invoke(arr);
				LogMgr.Log($"received packet\n{JsonSerializer.Serialize(packet, packet.GetType(), _options)}", TraceSourceType.PacketRecv);
				_packetHandlerQueue.Push(() => PacketHandler.HandlePacket(packet, session));
#else
				_packetHandlerQueue.Push(() => PacketHandler.HandlePacket(func.Invoke(buffer.Read(size)), session));
#endif
			}
		}
		public static bool WritePacket(this SendBuffer buffer, BasePacket packet)
		{
			if (BitConverter.TryWriteBytes(buffer.Write(2), packet.Id) == false) return false;
			try
			{
				var sizeSegment = buffer.Write(2);
				using (var writer = new Utf8JsonWriter(buffer))
				{
					JsonSerializer.Serialize(writer, packet, packet.GetType(), _options);
					LogMgr.Log($"Sending Packet {JsonSerializer.Serialize(packet, packet.GetType(), _options)}", TraceSourceType.PacketSend);
					writer.Flush();
					BitConverter.TryWriteBytes(sizeSegment, (ushort)writer.BytesCommitted);
				}
				return true;
			}
			catch (Exception ex) when (ex is InvalidOperationException or JsonException or ArgumentOutOfRangeException)
			{
				LogMgr.Log($"{ex.Message}", TraceEventType.Error, TraceSourceType.Error);
				return false;
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
