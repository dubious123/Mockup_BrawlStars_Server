
namespace ServerCore.Packets
{
	public class BasePacket
	{
		public ushort Id;
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
	public class C_EnterLobby : BasePacket
	{
		public C_EnterLobby()
		{
			Id = 0x0002;
		}
	}
	public class C_EnterGame : BasePacket
	{
		public C_EnterGame()
		{
			Id = 0x0003;
		}
		public ushort CharacterType;
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
		public S_EnterGame(bool result)
		{
			Id = 0x1003;
			Result = result;
		}
		public bool Result;
	}

}
