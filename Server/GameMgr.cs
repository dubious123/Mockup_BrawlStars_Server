using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using static Server.Utils.Enums;
using static ServerCore.Utils.Enums;

namespace Server
{
	public class GameMgr
	{
		static GameMgr _instance;
		ConcurrentDictionary<int, GameRoom> _roomDict;
		static int _roomCount;
		static GameMgr()
		{
			_instance = new GameMgr();
			_instance._roomDict = new ConcurrentDictionary<int, GameRoom>();
		}
		public static GameRoom FindWaitingGame()
		{
			var roomQuery = _instance._roomDict.Where(pair => pair.Value.State == GameState.Waiting);
			return roomQuery.ToArray().Length == 0 ? CreateGame() : roomQuery.First().Value;
		}
		public static GameRoom CreateGame()
		{
			var id = Interlocked.Increment(ref _roomCount);
			var room = new GameRoom(id, GameType.Team3vs3);
			_instance._roomDict.TryAdd(id, room);
			return room;
		}

		public static void EndGame(int id)
		{
			_instance._roomDict.TryRemove(id, out var room);
		}
		public static void EnterGame(int id, Player player)
		{
			_instance._roomDict.TryGetValue(id, out var room);
			room.Enter(player);
		}
		public static void EnterGame(GameRoom room, Player player)
		{
			room.Enter(player);
		}
	}
}
