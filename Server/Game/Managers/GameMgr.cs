using Server.Game;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using static Server.Utils.Enums;
using static ServerCore.Utils.Enums;

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
			Program.Update += () => _instance.Update();
		}
		public void Update()
		{
			foreach (var room in _roomDict.Values)
			{
				room.Broadcast();
			}
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
		public static short EnterGame(Player player)
		{
			return FindWaitingGame().Enter(player);
		}
		public static void EnterGame(int id, Player player)
		{
			_instance._roomDict.TryGetValue(id, out var room);
			room.Enter(player);
		}

		static GameRoom FindWaitingGame()
		{
			var roomQuery = _instance._roomDict.Where(pair => pair.Value.State == GameState.Waiting);
			return roomQuery.ToArray().Length == 0 ? CreateGame() : roomQuery.First().Value;
		}
	}
}
