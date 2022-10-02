using System;
using Server.Game;
using ServerCore;
using System.Numerics;
using System.Collections.Generic;


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
	public class C_GameReady : AuthPacket
	{
	}
	public class C_BroadcastPlayerInput : GamePacket
	{
		public byte ButtonPressed;
		public short TeamId;
		public long StartTick;
		public Vector2 MoveDir;
		public Vector2 LookDir;
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
		[Serializable]
		public struct PlayerInfoDto
		{
			public PlayerInfoDto(Player player)
			{
				CharacterType = player is null ? (ushort)0 : (ushort)player.Character.CharacterType;
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
	public class S_BroadcastStartGame : BasePacket
	{
		public S_BroadcastStartGame(float waitTime)
		{
			Id = 0x1005;
			WaitTime = waitTime;
		}
		public float WaitTime;
	}
	public class S_BroadcastGameState : BasePacket
	{
		public S_BroadcastGameState()
		{
			Id = 0x1006;
			PlayerMoveDirArr = new Vector2[6];
			PlayerLookDirArr = new Vector2[6];
			ButtonPressedArr = new ushort[6];
			Actions = new List<GameActionResult>();
		}
		public long StartTick;
		public long TargetTick;
		public Vector2[] PlayerMoveDirArr;
		public Vector2[] PlayerLookDirArr;
		public ushort[] ButtonPressedArr;
		public List<GameActionResult> Actions;

		[Serializable]
		public class GameActionResult
		{
			public GameActionResult(uint code, short subject, params short[] objects)
			{
				ActionCode = code;
				Subject = subject;
				Objects = objects;
			}
			//[][][][][][][][]
			//charType / action code (0,1,2,3...)
			public uint ActionCode;
			public short Subject;
			public short[] Objects;
		}



	}
	public class S_BroadcastMove : BasePacket
	{
		public S_BroadcastMove(short teamId, Vector2 moveDir, Vector2 lookDir)
		{
			Id = 0x1007;
			TeamId = teamId;
			MoveDir = moveDir;
			LookDir = lookDir;
		}
		public Vector2 MoveDir;
		public Vector2 LookDir;
		public short TeamId;
	}
}
