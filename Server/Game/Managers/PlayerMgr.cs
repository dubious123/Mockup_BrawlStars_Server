using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Managers
{
	public class PlayerMgr
	{
		static PlayerMgr _intstance = new();
		ConcurrentDictionary<int, Player> _playerDict;
		private PlayerMgr()
		{
			_playerDict = new();
		}
		public static Player GetOrAddPlayer(int userId)
		{
			if (_intstance._playerDict.TryGetValue(userId, out var player) == true)
			{
				return player;
			}
			player = new Player(userId);
			_intstance._playerDict.TryAdd(userId, player);
			return player;
		}
		public static Player GetPlayer(int userId)
		{
			_intstance._playerDict.TryGetValue(userId, out var player);
			return player;

		}
	}
}
