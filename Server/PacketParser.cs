using Server.Log;
using ServerCore;
using ServerCore.Packets;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using static Server.Utils.Enums;
using static ServerCore.Utils.Enums;

namespace Server
{
	public static class PacketParser
	{
		static readonly ConcurrentDictionary<ushort, Func<ArraySegment<byte>, BasePacket>> _readDict;
		static readonly JsonSerializerOptions _options;
		static PacketParser()
		{
#if DEBUG
			_options = new JsonSerializerOptions { IncludeFields = true, WriteIndented = true };
#else
			_options = new JsonSerializerOptions { IncludeFields = true };
#endif
			_readDict = new ConcurrentDictionary<ushort, Func<ArraySegment<byte>, BasePacket>>();
			_readDict.TryAdd((ushort)PacketId.C_Init, arr => JsonSerializer.Deserialize<C_Init>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.C_Login, arr => JsonSerializer.Deserialize<C_Login>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.C_EnterLobby, arr => JsonSerializer.Deserialize<C_EnterLobby>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.C_EnterGame, arr => JsonSerializer.Deserialize<C_EnterGame>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.C_BroadcastPlayerState, arr => JsonSerializer.Deserialize<C_BroadcastPlayerState>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.S_Init, arr => JsonSerializer.Deserialize<S_Init>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.S_Login, arr => JsonSerializer.Deserialize<S_Login>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.S_EnterLobby, arr => JsonSerializer.Deserialize<S_EnterLobby>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.S_EnterGame, arr => JsonSerializer.Deserialize<S_EnterGame>(arr, _options));
			_readDict.TryAdd((ushort)PacketId.S_BroadcastGameState, arr => JsonSerializer.Deserialize<S_BroadcastGameState>(arr, _options));
		}
		public static BasePacket ReadPacket(this RecvBuffer buffer)
		{
			try
			{
				var id = BitConverter.ToUInt16(buffer.Read(2));
				var size = BitConverter.ToUInt16(buffer.Read(2));
				_readDict.TryGetValue(id, out Func<ArraySegment<byte>, BasePacket> func);
#if DEBUG
				var packet = func.Invoke(buffer.Read(size));
				LogMgr.Log($"received packet {JsonSerializer.Serialize(packet, packet.GetType(), _options)}", TraceSourceType.PacketRecv);
				return packet;
#else
				return func.Invoke(buffer.Read(size));
#endif
			}
			catch (System.Exception)
			{
				throw;
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
