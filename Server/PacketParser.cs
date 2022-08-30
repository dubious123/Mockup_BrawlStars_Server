using ServerCore;
using ServerCore.Packets;
using ServerCore.Packets.Client;
using ServerCore.Packets.Server;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using static ServerCore.Utils.Enums;

namespace Server
{
	public static class PacketParser
	{
		static readonly ConcurrentDictionary<ushort, Func<ArraySegment<byte>, BasePacket>> _readDict;
		static PacketParser()
		{
			_readDict = new ConcurrentDictionary<ushort, Func<ArraySegment<byte>, BasePacket>>();
			_readDict.TryAdd((ushort)PacketId.S_Chat, arr => JsonSerializer.Deserialize<S_Chat>(arr));
			_readDict.TryAdd((ushort)PacketId.C_Chat, arr => JsonSerializer.Deserialize<C_Chat>(arr));
		}
		public static BasePacket ReadPacket(this RecvBuffer buffer)
		{
			try
			{
				var id = BitConverter.ToUInt16(buffer.Read(2));
				var size = BitConverter.ToUInt16(buffer.Read(2));
				_readDict.TryGetValue(id, out Func<ArraySegment<byte>, BasePacket> func);
				return func.Invoke(buffer.Read(size));
			}
			catch (System.Exception)
			{
				return null;
			}
		}
		public static bool WritePacket(this SendBuffer buffer, BasePacket packet)
		{
			try
			{
				BitConverter.TryWriteBytes(buffer.Write(2), packet.Id);
				var sizeSegment = buffer.Write(2);
				using (var writer = new Utf8JsonWriter(buffer))
				{
					JsonSerializer.Serialize(writer, packet);
					writer.Flush();
					BitConverter.TryWriteBytes(sizeSegment, writer.BytesCommitted);
				}
				return true;
			}
			catch (System.Exception)
			{
				return false;
			}
		}
	}
}
