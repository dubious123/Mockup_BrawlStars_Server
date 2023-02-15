namespace Server
{
	public static class PacketParser
	{
		private static readonly ConcurrentDictionary<ushort, Func<ArraySegment<byte>, BasePacket>> _readDict;
		private static readonly JsonSerializerOptions _options;
		//private static JobQueue _packetHandlerQueue;
		static PacketParser()
		{
			//_packetHandlerQueue = JobMgr.GetQueue(Define.PacketQueueName);
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
			_readDict.TryAdd((ushort)PacketId.C_PlayerInput, arr => JsonSerializer.Deserialize<C_PlayerInput>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.S_Init, arr => JsonSerializer.Deserialize<S_Init>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.S_Login, arr => JsonSerializer.Deserialize<S_Login>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.S_EnterLobby, arr => JsonSerializer.Deserialize<S_EnterLobby>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.S_EnterGame, arr => JsonSerializer.Deserialize<S_EnterGame>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.S_BroadcastFoundPlayer, arr => JsonSerializer.Deserialize<S_BroadcastFoundPlayer>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.S_BroadcastStartGame, arr => JsonSerializer.Deserialize<S_BroadcastStartGame>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.S_GameFrameInfo, arr => JsonSerializer.Deserialize<S_GameFrameInfo>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.S_BroadcastMatchOver, arr => JsonSerializer.Deserialize<S_BroadcastMatchOver>(arr, _options));
		}

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
				var packet = func.Invoke(buffer.Read(size));
				//_packetHandlerQueue.Push(() => PacketHandler.HandlePacket(packet, session));
				Loggers.Recv.Information("Recv Packet {0}", JsonSerializer.Serialize(packet, packet.GetType(), _options));
				PacketHandler.HandlePacket(packet, session);
#else
				var packet = func.Invoke(buffer.Read(size));
				Loggers.Recv.Information("Recv Packet {0}", JsonSerializer.Serialize(packet, packet.GetType(), _options));
				//_packetHandlerQueue.Push(() => );
				PacketHandler.HandlePacket(packet, session);
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
					Loggers.Send.Information("Sending Packet {0}", JsonSerializer.Serialize(packet, packet.GetType(), _options));
					writer.Flush();
					BitConverter.TryWriteBytes(sizeSegment, (ushort)writer.BytesCommitted);
				}

				return true;
			}
			catch (Exception ex) when (ex is InvalidOperationException or JsonException or ArgumentOutOfRangeException)
			{
				Loggers.Error.Error(ex.Message);
				return false;
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
