using System.Collections.Concurrent;
using static Server.Utils.Enums;

namespace Server
{
	public class GameRoom
	{
		public int Id;
		public GameType Type;
		public GameState State;
		ConcurrentDictionary<int, Player> _playerDict;
		public GameRoom(int id, GameType type)
		{
			Id = id;
			_playerDict = new ConcurrentDictionary<int, Player>();
			Type = type;
			State = GameState.Waiting;
		}
		public void Enter(Player player)
		{
			_playerDict.TryAdd(player.PlayerId, player);
		}
	}
}
