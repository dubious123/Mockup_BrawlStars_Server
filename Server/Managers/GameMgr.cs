

namespace Server;

public class GameMgr
{
	private static int _roomCount;
	private static GameMgr _instance = new();
	private ConcurrentDictionary<int, GameRoom> _roomDict;

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
		FindWaitingGame().Enter(player);
	}

	private static GameRoom FindWaitingGame()
	{
		var roomQuery = _instance._roomDict.Where(pair => pair.Value.State == GameState.Waiting);
		return roomQuery.ToArray().Length == 0 ? CreateGame() : roomQuery.First().Value;
	}
}
