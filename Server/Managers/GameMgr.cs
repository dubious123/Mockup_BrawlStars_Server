

using System.Timers;

namespace Server;

public class GameMgr
{
	private static int _roomCount;
	private static GameMgr _instance = new();
	private ConcurrentDictionary<int, GameRoom> _roomDict;
	private ConcurrentBag<GameLoopQueue> _loopQueueBag;
	private GameMgr()
	{
		_roomDict = new();
		_loopQueueBag = new();
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

	public static void StartGame(GameRoom room)
	{
		foreach (var queue in _instance._loopQueueBag)
		{
			if (queue.IsFull is false)
			{
				queue.Push(room);
				return;
			}
		}

		var newLoop = new GameLoopQueue();
		newLoop.Push(room);
		_instance._loopQueueBag.Add(newLoop);
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
		var roomQuery = _instance._roomDict.Where(pair => pair.Value.GameStarted is false);
		return roomQuery.ToArray().Length == 0 ? CreateGame() : roomQuery.First().Value;
	}
}
