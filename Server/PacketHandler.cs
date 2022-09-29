using System;
using System.Collections.Concurrent;
using ServerCore;
using Microsoft.EntityFrameworkCore;
using Server.DB;
using System.Linq;
using Server.Game.Managers;
using static ServerCore.Utils.Enums;
using Server.Log;
using static Server.Utils.Enums;
using System.Diagnostics;
using System.Numerics;

namespace Server
{
	public static class PacketHandler
	{
		static ConcurrentDictionary<PacketId, Action<BasePacket, ClientSession>> _handlerDict;
		static PacketHandler()
		{
			_handlerDict = new ConcurrentDictionary<PacketId, Action<BasePacket, ClientSession>>();
			_handlerDict.TryAdd(PacketId.C_Init, (packet, session) => C_InitHandle(packet, session));
			_handlerDict.TryAdd(PacketId.C_Login, (packet, session) => C_LoginHandle(packet, session));
			_handlerDict.TryAdd(PacketId.C_EnterLobby, (packet, session) => C_EnterLobbyHandle(packet, session));
			_handlerDict.TryAdd(PacketId.C_EnterGame, (packet, session) => C_EnterGameHandle(packet, session));
			_handlerDict.TryAdd(PacketId.C_GameReady, (packet, session) => C_GameReadyHandle(packet, session));
			_handlerDict.TryAdd(PacketId.C_BroadcastPlayerInput, (packet, session) => C_BroadcastPlayerInputHandle(packet, session));
		}

		public static void HandlePacket(BasePacket packet, ClientSession session)
		{
			if (packet == null) return;
			if (_handlerDict.TryGetValue((PacketId)packet.Id, out Action<BasePacket, ClientSession> action) == false)
			{
				LogMgr.Log($"Invalid Packet {packet}", TraceEventType.Error, TraceSourceType.Console, TraceSourceType.PacketHandler);
				throw new Exception();
			}
			action.Invoke(packet, session);
		}


		private static void C_InitHandle(BasePacket packet, ClientSession session)
		{
			var req = packet as C_Init;
			session.RegisterSend(new S_Init());
		}

		private static void C_LoginHandle(BasePacket packet, ClientSession session)
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
				return;
			}
			session.RegisterSend(new S_Login { result = false });
		}

		private static void C_EnterLobbyHandle(BasePacket packet, ClientSession session)
		{
			var req = packet as C_EnterLobby;

			session.RegisterSend(new S_EnterLobby());
		}

		private static void C_EnterGameHandle(BasePacket packet, ClientSession session)
		{
			var req = packet as C_EnterGame;
			using GameDBContext db = new();
			var res = db.Users
				.AsNoTracking()
				.Any(i => i.UserId == req.UserId);
			if (res == false)
			{
				throw new Exception();
			}
			GameMgr.EnterGame(PlayerMgr.GetOrAddPlayer(req.UserId, session));
		}

		private static void C_GameReadyHandle(BasePacket packet, ClientSession session)
		{
			var req = packet as C_GameReady;
			var player = PlayerMgr.GetPlayer(req.UserId);
			Debug.Assert(player is not null);
			player.GameSceneReady = true;
			if (player.CurrentGame.IsReady == false) return;
			player.CurrentGame.StartGame();
		}

		private static void C_BroadcastPlayerInputHandle(BasePacket packet, ClientSession session)
		{
			var req = packet as C_BroadcastPlayerInput;
			using GameDBContext db = new();
			var player = PlayerMgr.GetPlayer(req.UserId);


			if (player == null || player.CurrentGame == null)
			{
				return;
			}
			var input = new Game.PlayerInput()
			{
				ClientTargetTick = req.StartTick + 6,
				ReceivedTick = player.CurrentGame.CurrentTick,
				MoveDirX = req.MoveDir.X,
				MoveDirY = req.MoveDir.Y,
				LookDirX = req.LookDir.X,
				LookDirY = req.LookDir.Y,
				ButtonPressed = req.ButtonPressed,
			};
			player.InputBuffer.Enqueue(input);

			return;
		}

	}
}
