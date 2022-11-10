namespace Server.Utils;

public partial class Enums
{
	public enum GameType
	{
		Team3vs3,
	}

	public enum GameState
	{
		Waiting,
		Started,
		Ended,
	}

	public enum CharacterType
	{
		None,
		Knight,
	}

	public enum TileType
	{
		Emtpy,
		Wall,
	}

	public enum TraceSourceType
	{
		Packet,
		Network,
		Session,
		PacketHandler,
		Console,
		Error,
		PacketSend,
		PacketRecv,
		Debug,
		Game,
	}
}
