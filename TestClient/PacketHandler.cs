using ServerCore;
using ServerCore.Packets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using static ServerCore.Utils.Enums;

namespace TestClient
{
	public static class PacketHandler
	{
		static ConcurrentDictionary<PacketId, Action<BasePacket>> _handlerDict;
		static PacketHandler()
		{
			_handlerDict = new ConcurrentDictionary<PacketId, Action<BasePacket>>();
			_handlerDict.TryAdd(PacketId.S_Chat, packet => Console.WriteLine("Handle S_Chat"));
		}

		public static void HandlePackets(List<BasePacket> packets)
		{
			foreach (var packet in packets)
			{
				if (_handlerDict.TryGetValue((PacketId)packet.Id, out Action<BasePacket> action) == false)
					throw new Exception();
				action.Invoke(packet);
			}
		}
	}
}
