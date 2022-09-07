
namespace ServerCore.Packets
{
	public class BasePacket
	{
		public ushort Id;
	}
	public class AuthPacket : BasePacket
	{
		public int UserId;
	}
	public class GamePacket : AuthPacket
	{
		public int RoomId;
		public ushort CharacterType;
	}

	public class C_Init : BasePacket
	{
		public C_Init()
		{
			Id = 0x0000;
		}
	}
	public class C_Login : BasePacket
	{
		public C_Login()
		{
			Id = 0x0001;
		}
		public string loginId;
		public string loginPw;
	}
	public class C_EnterLobby : AuthPacket
	{
		public C_EnterLobby()
		{
			Id = 0x0002;
		}
	}
	public class C_EnterGame : AuthPacket
	{
		public C_EnterGame()
		{
			Id = 0x0003;
		}
		public ushort CharacterType;
	}
	public class C_BroadcastPlayerState : GamePacket
	{
		public C_BroadcastPlayerState(int userId)
		{
			Id = 0x0004;
			UserId = userId;
		}
		public float PosX;
		public float PosY;
		public float LookDirX;
		public float LookDirY;
	}


	public class S_Init : BasePacket
	{
		public S_Init()
		{
			Id = 0x1000;
		}
	}
	public class S_Login : BasePacket
	{
		public S_Login()
		{
			Id = 0x1001;
		}
		public bool result;
		public int userId;
	}
	public class S_EnterLobby : BasePacket
	{
		public S_EnterLobby()
		{
			Id = 0x1002;
		}
	}
	public class S_EnterGame : BasePacket
	{
		public S_EnterGame(bool result, int roomId)
		{
			Id = 0x1003;
			Result = result;
			RoomId = roomId;
		}
		public bool Result;
		public int RoomId;
	}
	public class S_BroadcastGameState : BasePacket
	{
		public S_BroadcastGameState(bool result)
		{
			Id = 0x1004;
			Result = result;
		}
		public bool Result;
	}

}
