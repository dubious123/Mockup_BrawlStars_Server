using ServerCore;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using static ServerCore.Utils.Enums;
using Google.Protobuf;

namespace Server
{
	public static class PacketHandler
	{
		static ConcurrentDictionary<ushort, Action<IMessage, Session>> _handlerDict;
		static PacketHandler()
		{
			_handlerDict = new ConcurrentDictionary<ushort, Action<IMessage, Session>>();
			_handlerDict.TryAdd((ushort)PacketId.C_Chat, (p, s) => Console.WriteLine("Handle C_Chat"));
		}

		public static void HandlePacket<T, P>(T packet, P session) where T : IMessage where P : Session
		{
			if (_handlerDict.TryGetValue(packet.Id, out Action<IMessage, Session> action) == false)
				throw new Exception();
			action.Invoke(packet, session);
		}
	}
}
