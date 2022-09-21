using Server.Game;
using ServerCore;
using System.Numerics;


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
	public class C_BroadcastPlayerState : GamePacket
	{
		public short TeamId;
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
		public S_EnterGame(short teamId, Player[] playerInfoArr)
		{
			Id = 0x1003;
			TeamId = teamId;
			PlayerInfoArr = new PlayerInfoDto[playerInfoArr.Length];
			for (int i = 0; i < playerInfoArr.Length; i++)
			{
				PlayerInfoArr[i] = new PlayerInfoDto(playerInfoArr[i]);
			}
		}
		public struct PlayerInfoDto
		{
			public PlayerInfoDto(Player player)
			{
				CharacterType = player is null ? (ushort)0 : (ushort)player.CharacterType;
			}
			public ushort CharacterType;
		}
		public short TeamId;
		public PlayerInfoDto[] PlayerInfoArr;
	}
	public class S_BroadcastEnterGame : BasePacket
	{
		public S_BroadcastEnterGame(ushort characterType, short teamId)
		{
			Id = 0x1004;
			Charactertype = characterType;
			TeamId = teamId;
		}
		public ushort Charactertype;
		public short TeamId;
	}
	public class S_BroadcastGameState : BasePacket
	{
		public S_BroadcastGameState(int roomId, ushort playerCount)
		{
			Id = 0x1005;
			PlayerPosArr = new Vector2[6];
			PlayerLookDirArr = new Vector2[6];
			RoomId = roomId;
			PlayerCount = playerCount;
		}
		public int RoomId;
		public Vector2[] PlayerPosArr;
		public Vector2[] PlayerLookDirArr;

		public ushort PlayerCount;


	}
}
