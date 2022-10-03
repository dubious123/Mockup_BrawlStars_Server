namespace Server.Game
{
	public class Player
	{
		public int UserId { get; init; }
		public short TeamId { get; set; }
		public bool GameSceneReady { get; set; } = false;
		public ClientSession Session { get; init; }
		public GameRoom CurrentGame;
		public BaseCharacter Character { get; set; }
		public ConcurrentQueue<PlayerInput> InputBuffer { get; set; } = new();

		public Player(int userId, ClientSession session)
		{
			UserId = userId;
			Session = session;
		}
	}
}
