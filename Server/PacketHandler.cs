using System;
using System.Collections.Concurrent;
using static ServerCore.Utils.Enums;
using ServerCore;
using System.Collections.Generic;
using ServerCore.Packets;

namespace Server
{
	public static class PacketHandler
	{
		static ConcurrentDictionary<PacketId, Action<BasePacket>> _handlerDict;
		static PacketHandler()
		{
			_handlerDict = new ConcurrentDictionary<PacketId, Action<BasePacket>>();
			_handlerDict.TryAdd(PacketId.C_Chat, packet => Console.WriteLine("Handle C_Chat"));
		}

		public static void HandlePacket(BasePacket packet)
		{
			if (packet == null) return;
			if (_handlerDict.TryGetValue((PacketId)packet.Id, out Action<BasePacket> action) == false)
				throw new Exception();
			action.Invoke(packet);
		}
	}
}
