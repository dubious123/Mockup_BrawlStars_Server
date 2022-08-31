
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
			Id = 0;
		}
	}

	public class C_EnterGame : BasePacket
	{
		public C_EnterGame()
		{
			Id = 1;
		}
	}

	public class C_EnterLobby : BasePacket
	{
		public C_EnterLobby()
		{
			Id = 2;
		}
	}

	public class S_Chat : BasePacket
	{
		public S_Chat()
		{
			Id = 3;
		}
	}
	public class S_EnterLobby : BasePacket
	{
		public S_EnterLobby()
		{
			Id = 4;
		}
	}

}
