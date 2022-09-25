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
			C_BroadcastPlayerInput = 0x0004,
			S_Init = 0x1000,
			S_Login = 0x1001,
			S_EnterLobby = 0x1002,
			S_EnterGame = 0x1003,
			S_BroadcastEnterGame = 0x1004,
			S_BroadcastGameState = 0x1005,
			S_BroadcastMove = 0x1006,
		}

	}
}
