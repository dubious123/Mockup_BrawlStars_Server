namespace Server.Game
{
	public class Player
	{

		public int UserId { get; init; }
		public short TeamId { get; set; }
		public bool GameSceneReady { get; set; }
		public ClientSession Session { get; init; }
		public GameRoom CurrentGame { get; set; }
		public NetObjectType CharacterType { get; set; }
		public NetCharacter Character { get; set; }
		public ConcurrentQueue<InputData> InputBuffer { get; set; } = new();

		public Player(int userId, ClientSession session)
		{
			UserId = userId;
			Session = session;
			GameSceneReady = false;
		}
	}
}
