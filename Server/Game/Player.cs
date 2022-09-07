using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
	public class Player
	{
		public int UserId { get; init; }
		public GameRoom CurrentGame;
		Transform _transform = new();

		public Player(int userId)
		{
			UserId = userId;
		}
	}
}
