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
			_handlerDict.TryAdd(PacketId.C_Chat, packet => C_ChatHandle(packet));
			_handlerDict.TryAdd(PacketId.C_EnterGame, packet => C_EnterGameHandle(packet));
			_handlerDict.TryAdd(PacketId.C_EnterLobby, packet => C_EnterLobbyHandle(packet));
		}

		public static void HandlePacket(BasePacket packet)
		{
			if (packet == null) return;
			if (_handlerDict.TryGetValue((PacketId)packet.Id, out Action<BasePacket> action) == false)
				throw new Exception();
			action.Invoke(packet);
		}

		private static void C_ChatHandle(BasePacket packet)
		{
			packet = packet as C_Chat;
			Console.WriteLine("Handle C_Chat");
		}

		private static void C_EnterGameHandle(BasePacket packet)
		{
			packet = packet as C_EnterGame;
		}

		private static void C_EnterLobbyHandle(BasePacket packet)
		{
			packet = packet as C_EnterLobby;
		}
	}
}
