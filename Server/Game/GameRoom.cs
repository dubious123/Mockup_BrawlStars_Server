using Server.Game;
using System.Collections.Concurrent;
using static Server.Utils.Enums;

namespace Server
{
	public class GameRoom
	{
		public int Id;
		public ushort MapId;
		public GameType Type;
		public GameState State;
		ConcurrentDictionary<int, Player> _playerDict;
		public GameRoom(int id, GameType type, ushort mapId)
		{
			Id = id;
			_playerDict = new ConcurrentDictionary<int, Player>();
			Type = type;
			State = GameState.Waiting;
			MapId = mapId;
		}
		public int Enter(Player player)
		{
			_playerDict.TryAdd(player.UserId, player);
			player.CurrentGame = this;
			return Id;
		}
	}
}
