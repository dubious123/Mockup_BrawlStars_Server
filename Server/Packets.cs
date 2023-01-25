

namespace Server
{
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
	}
	public class C_SyncTime : BasePacket
	{

		public long ClientLocalTime;
	}
	public class C_Login : BasePacket
	{

		public string loginId;
		public string loginPw;
	}
	public class C_EnterLobby : AuthPacket
	{
	}
	public class C_EnterGame : AuthPacket
	{

		public ushort CharacterType;
	}
	public class C_GameReady : AuthPacket
	{
	}
	public class C_PlayerInput : GamePacket
	{

		public byte ButtonPressed;
		public short TeamId;
		public int FrameNum;
		public uint MoveDirX;
		public uint MoveDirY;
		public uint LookDirX;
		public uint LookDirY;
	}
	public class S_Init : BasePacket
	{
		public S_Init()
		{
			Id = 0x1000;
		}
	}
	public class S_SyncTime : BasePacket
	{
		public S_SyncTime()
		{
			Id = 0x1001;
		}
		public long ClientLocalTime;
		public long ServerTime;
	}
	public class S_Login : BasePacket
	{
		public S_Login()
		{
			Id = 0x1002;
		}
		public bool result;
		public int userId;
	}
	public class S_EnterLobby : BasePacket
	{
		public S_EnterLobby()
		{
			Id = 0x1003;
		}
	}
	public class S_EnterGame : BasePacket
	{
		public S_EnterGame(short teamId, Player player)
		{
			Id = 0x1004;
			TeamId = teamId;
			PlayerInfo = new PlayerInfoDto(player);
		}

		[Serializable]
		public struct PlayerInfoDto
		{
			public PlayerInfoDto(Player player)
			{
				CharacterType = player is null ? (ushort)0 : (ushort)player.CharacterType;
			}

			public ushort CharacterType;
		}

		public short TeamId;
		public PlayerInfoDto PlayerInfo;
	}
	public class S_BroadcastFoundPlayer : BasePacket
	{
		public S_BroadcastFoundPlayer(int foundPlayersCount)
		{
			Id = 0x1005;
			FoundPlayersCount = foundPlayersCount;
		}

		public int FoundPlayersCount;
	}
	public class S_BroadcastEnterGame : BasePacket
	{
		public S_BroadcastEnterGame(ushort characterType, short teamId)
		{
			Id = 0x1006;
			Charactertype = characterType;
			TeamId = teamId;
		}

		public ushort Charactertype;
		public short TeamId;
	}
	public class S_BroadcastStartGame : BasePacket
	{
		public S_BroadcastStartGame()
		{
			Id = 0x1007;
			CharacterTypeArr = new ushort[6];
		}

		public ushort[] CharacterTypeArr;
	}
	public class S_GameFrameInfo : BasePacket
	{
		public S_GameFrameInfo()
		{
			Id = 0x1008;
			PlayerMoveDirXArr = new uint[6];
			PlayerMoveDirYArr = new uint[6];
			PlayerLookDirXArr = new uint[6];
			PlayerLookDirYArr = new uint[6];
			ButtonPressedArr = new ushort[6];
		}

		public int FrameNum;
		public uint[] PlayerMoveDirXArr;
		public uint[] PlayerMoveDirYArr;
		public uint[] PlayerLookDirXArr;
		public uint[] PlayerLookDirYArr;
		public ushort[] ButtonPressedArr;
	}
	public class S_BroadcastMatchOver : BasePacket
	{
		public S_BroadcastMatchOver()
		{
			Id = 0x1009;
		}
	}
}
