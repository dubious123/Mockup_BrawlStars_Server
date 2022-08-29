using System;
using System.Collections.Concurrent;
using static ServerCore.Utils.Enums;
using Google.Protobuf;
using ServerCore;
using System.Collections.Generic;

namespace Server
{
	public static class PacketHandler
	{
		static ConcurrentDictionary<PacketId, Action<IMessage>> _handlerDict;
		static PacketHandler()
		{
			_handlerDict = new ConcurrentDictionary<PacketId, Action<IMessage>>();
			_handlerDict.TryAdd(PacketId.C_Chat, packet => Console.WriteLine("Handle C_Chat"));
		}

		public static void HandlePackets(List<PacketContext> packets)
		{
			foreach (var packet in packets)
			{
				if (_handlerDict.TryGetValue(packet.Id, out Action<IMessage> action) == false)
					throw new Exception();
				action.Invoke(packet.Message);
			}
		}
	}
}
