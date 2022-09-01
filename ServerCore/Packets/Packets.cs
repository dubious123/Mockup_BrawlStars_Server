
namespace ServerCore.Packets
{
	public class BasePacket
	{
		public ushort Id;
	}

	public class C_Chat : BasePacket
	{
		public C_Chat()
		{
			Id = 0x0000;
		}
	}
	public class C_EnterLobby : BasePacket
	{
		public C_EnterLobby()
		{
			Id = 0x0001;
		}
	}
	public class C_EnterGame : BasePacket
	{
		public C_EnterGame()
		{
			Id = 0x0002;
		}
		public ushort CharacterType;
	}



	public class S_Chat : BasePacket
	{
		public S_Chat()
		{
			Id = 0x1000;
		}
	}
	public class S_EnterLobby : BasePacket
	{
		public S_EnterLobby()
		{
			Id = 0x1001;
		}
	}
	public class S_EnterGame : BasePacket
	{
		public S_EnterGame(bool result)
		{
			Id = 0x1002;
			Result = result;
		}
		public bool Result;
	}

}
