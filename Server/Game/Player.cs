using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Server.Utils;
using Server.Game.Base;
using System.Collections.Concurrent;

namespace Server.Game
{
	public class Player
	{
		public int UserId { get; init; }
		public short TeamId { get; set; }
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
