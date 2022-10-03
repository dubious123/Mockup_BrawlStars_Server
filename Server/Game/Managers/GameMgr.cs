namespace Server
{
	public class GameMgr
	{
		static GameMgr _instance = new();
		ConcurrentDictionary<int, GameRoom> _roomDict;
		static int _roomCount;
		private GameMgr()
		{
			_roomDict = new();
		}
		public static void Init()
		{
		}
		public static GameRoom CreateGame()
		{
			var id = Interlocked.Increment(ref _roomCount);
			var room = new GameRoom(id, 0);
			_instance._roomDict.TryAdd(id, room);
			return room;
		}

		public static void EndGame(int id)
		{
			_instance._roomDict.TryRemove(id, out var room);
		}
		public static void EnterGame(Player player)
		{
			var game = FindWaitingGame();
			player.Session.RegisterSend(new S_EnterGame(game.Enter(player), game.Players));
		}

		static GameRoom FindWaitingGame()
		{
			var roomQuery = _instance._roomDict.Where(pair => pair.Value.State == GameState.Waiting);
			return roomQuery.ToArray().Length == 0 ? CreateGame() : roomQuery.First().Value;
		}
	}
}
