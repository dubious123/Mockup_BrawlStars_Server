namespace ServerCore.Utils
{
	public static class Enums
	{
		public enum PacketId
		{
			C_Init = 0x0000,
			C_Login = 0x0001,
			C_EnterLobby = 0x0002,
			C_EnterGame = 0x0003,
			C_GameReady = 0x0004,
			C_PlayerInput = 0x0005,
			S_Init = 0x1000,
			S_Login = 0x1001,
			S_EnterLobby = 0x1002,
			S_GameReady = 0x1003,
			S_EnterGame = 0x1004,
			S_BroadcastSearchPlayer = 0x1005,
			S_BroadcastEnterGame = 0x1006,
			S_BroadcastStartGame = 0x1007,
			S_GameFrameInfo = 0x1008,
		}

	}
}
