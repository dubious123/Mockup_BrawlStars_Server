using Google.Protobuf;
using ServerCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using static ServerCore.Utils.Enums;

namespace TestClient
{
	public static class PacketHandler
	{
		static ConcurrentDictionary<PacketId, Action<IMessage>> _handlerDict;
		static PacketHandler()
		{
			_handlerDict = new ConcurrentDictionary<PacketId, Action<IMessage>>();
			_handlerDict.TryAdd(PacketId.S_Chat, packet => Console.WriteLine("Handle S_Chat"));
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
