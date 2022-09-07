using System;
using System.Collections.Concurrent;
using ServerCore;
using ServerCore.Packets;
using Microsoft.EntityFrameworkCore;
using Server.DB;
using System.Linq;
using Server.Game.Managers;
using static ServerCore.Utils.Enums;

namespace Server
{
	public static class PacketHandler
	{
		static ConcurrentDictionary<PacketId, Action<BasePacket, Session>> _handlerDict;
		static PacketHandler()
		{
			_handlerDict = new ConcurrentDictionary<PacketId, Action<BasePacket, Session>>();
			_handlerDict.TryAdd(PacketId.C_Init, (packet, session) => C_InitHandle(packet, session));
			_handlerDict.TryAdd(PacketId.C_Login, (packet, session) => C_LoginHandle(packet, session));
			_handlerDict.TryAdd(PacketId.C_EnterLobby, (packet, session) => C_EnterLobbyHandle(packet, session));
			_handlerDict.TryAdd(PacketId.C_EnterGame, (packet, session) => C_EnterGameHandle(packet, session));
			_handlerDict.TryAdd(PacketId.C_BroadcastPlayerState, (packet, session) => C_BroadcastPlayerStateHandle(packet, session));
		}

		public static void HandlePacket(BasePacket packet, Session session)
		{
			if (packet == null) return;
			if (_handlerDict.TryGetValue((PacketId)packet.Id, out Action<BasePacket, Session> action) == false)
				throw new Exception();
			action.Invoke(packet, session);
		}


		private static void C_InitHandle(BasePacket packet, Session session)
		{
			var req = packet as C_Init;
			session.RegisterSend(new S_Init());
			session.Send();
		}

		private static void C_LoginHandle(BasePacket packet, Session session)
		{
			var req = packet as C_Login;
			using GameDBContext db = new();
			var userId = db.Users
				.AsNoTracking()
				.Where(u => u.LoginId == req.loginId && u.LoginPw == req.loginPw)
				.Select(u => u.UserId)
				.FirstOrDefault();
			if (userId != 0)
			{
				session.RegisterSend(new S_Login { result = true, userId = userId });
				session.Send();
				return;
			}
			session.RegisterSend(new S_Login { result = false });
			session.Send();
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
			using GameDBContext db = new();
			var res = db.Users
				.AsNoTracking()
				.Any(i => i.UserId == req.UserId);
			if (res == false)
			{
				session.RegisterSend(new S_EnterGame(false, 0));
				session.Send();
				return;
			}
			var roomId = GameMgr.EnterGame(PlayerMgr.GetOrAddPlayer(req.UserId));

			session.RegisterSend(new S_EnterGame(true, roomId));
			session.Send();
		}

		private static void C_BroadcastPlayerStateHandle(BasePacket packet, Session session)
		{
			var req = packet as C_BroadcastPlayerState;
			using GameDBContext db = new();
			var player = PlayerMgr.GetPlayer(req.UserId);

			if (player == null)
			{
				session.RegisterSend(new S_BroadcastGameState(false));
				session.Send();
				return;
			}
			bool res = MapMgr.GetMapData(player.CurrentGame.MapId).CanGo(new System.Numerics.Vector2(req.PosX, req.PosY));

			session.RegisterSend(new S_BroadcastGameState(res));
			session.Send();
			return;
		}
	}
}
