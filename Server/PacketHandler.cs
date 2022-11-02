


using Server.Logs;

namespace Server
{
	public static class PacketHandler
	{
		private static ConcurrentDictionary<PacketId, Action<BasePacket, ClientSession>> _handlerDict;
		static PacketHandler()
		{
			_handlerDict = new ConcurrentDictionary<PacketId, Action<BasePacket, ClientSession>>();
			_handlerDict.TryAdd(PacketId.C_Init, (packet, session) => C_InitHandle(packet, session));
			_handlerDict.TryAdd(PacketId.C_Login, (packet, session) => C_LoginHandle(packet, session));
			_handlerDict.TryAdd(PacketId.C_EnterLobby, (packet, session) => C_EnterLobbyHandle(packet, session));
			_handlerDict.TryAdd(PacketId.C_EnterGame, (packet, session) => C_EnterGameHandle(packet, session));
			_handlerDict.TryAdd(PacketId.C_GameReady, (packet, session) => C_GameReadyHandle(packet, session));
			_handlerDict.TryAdd(PacketId.C_PlayerInput, (packet, session) => C_PlayerInputHandle(packet, session));
		}

		public static void HandlePacket(BasePacket packet, ClientSession session)
		{
			if (packet == null) return;
			if (_handlerDict.TryGetValue((PacketId)packet.Id, out Action<BasePacket, ClientSession> action) == false)
			{
				Loggers.Error.Fatal("Invalid Packet {packet}", packet);
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

			var player = PlayerMgr.GetOrAddPlayer(req.UserId, session);
			player.CharType = (CharacterType)req.CharacterType;
			GameMgr.EnterGame(player);
		}

		private static void C_GameReadyHandle(BasePacket packet, ClientSession session)
		{
			var req = packet as C_GameReady;
			var player = PlayerMgr.GetPlayer(req.UserId);
			Debug.Assert(player is not null);
			player.GameSceneReady = true;
		}

		private static void C_PlayerInputHandle(BasePacket packet, ClientSession session)
		{
			var req = packet as C_PlayerInput;
			using GameDBContext db = new();
			var player = PlayerMgr.GetPlayer(req.UserId);
			if (player == null || player.CurrentGame == null)
			{
				return;
			}

			var input = new InputData()
			{
				MoveInput = new sVector3(sfloat.FromRaw(req.MoveDirX), (sfloat)0f, sfloat.FromRaw(req.MoveDirY)),
				LookInput = new sVector3(sfloat.FromRaw(req.LookDirX), (sfloat)0f, sfloat.FromRaw(req.LookDirY)),
				ButtonInput = req.ButtonPressed,
			};

			player.InputBuffer.Enqueue(input);
			return;
		}
	}
}
