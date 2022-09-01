using System;
using System.Collections.Concurrent;
using static ServerCore.Utils.Enums;
using ServerCore;
using System.Collections.Generic;
using ServerCore.Packets;
using Server.Utils;

namespace Server
{
	public static class PacketHandler
	{
		static ConcurrentDictionary<PacketId, Action<BasePacket, Session>> _handlerDict;
		static PacketHandler()
		{
			_handlerDict = new ConcurrentDictionary<PacketId, Action<BasePacket, Session>>();
			_handlerDict.TryAdd(PacketId.C_Chat, (packet, session) => C_ChatHandle(packet, session));
			_handlerDict.TryAdd(PacketId.C_EnterLobby, (packet, session) => C_EnterLobbyHandle(packet, session));
			_handlerDict.TryAdd(PacketId.C_EnterGame, (packet, session) => C_EnterGameHandle(packet, session));
		}

		public static void HandlePacket(BasePacket packet, Session session)
		{
			if (packet == null) return;
			if (_handlerDict.TryGetValue((PacketId)packet.Id, out Action<BasePacket, Session> action) == false)
				throw new Exception();
			action.Invoke(packet, session);
		}


		private static void C_ChatHandle(BasePacket packet, Session session)
		{
			var req = packet as C_Chat;
		}

		private static void C_EnterLobbyHandle(BasePacket packet, Session session)
		{
			var req = packet as C_EnterLobby;

			session.RegisterSend(new S_EnterLobby());
			session.Send();
		}

		private static void C_EnterGameHandle(BasePacket packet, Session session)
		{
			var req = packet as C_EnterGame;

			var room = GameMgr.FindWaitingGame();
			room.EnterGame(new Player { CharType = (CharacterType)req.CharacterType, Id = 1 });

			session.RegisterSend(new S_EnterGame(true));
			session.Send();
		}


	}
}
