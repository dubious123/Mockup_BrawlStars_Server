using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ServerCore.Utils.Enums;

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
			_playerDict.TryAdd(player.Id, player);
		}
	}
}
