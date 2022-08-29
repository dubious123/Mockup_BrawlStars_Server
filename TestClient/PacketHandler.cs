using ServerCore;
using ServerCore.Packets;
using System;
using System.Collections.Concurrent;
using static ServerCore.Utils.Enums;

namespace TestClient
{
	public static class PacketHandler
	{
		static ConcurrentDictionary<ushort, Action<BasePacket, Session>> _handlerDict;
		static PacketHandler()
		{
			_handlerDict = new ConcurrentDictionary<ushort, Action<BasePacket, Session>>();
			_handlerDict.TryAdd((ushort)PacketId.S_Chat, (p, s) => Console.WriteLine("Handle S_Chat"));
		}

		public static void HandlePacket<T, P>(T packet, P session) where T : BasePacket where P : Session
		{
			if (_handlerDict.TryGetValue(packet.Id, out Action<BasePacket, Session> action) == false)
				throw new Exception();
			action.Invoke(packet, session);
		}
	}
}
