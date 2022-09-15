using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Server.Utils;

namespace Server.Game
{
	public class Player
	{
		public int UserId { get; init; }
		public short TeamId { get; set; }
		public Vector2 Position { get; set; }
		public Vector2 LookDir { get; set; }
		public ClientSession Session { get; init; }
		public GameRoom CurrentGame;
		public Enums.CharacterType CharacterType { get; set; } = 0;

		public Player(int userId, ClientSession session)
		{
			UserId = userId;
			Session = session;
		}
	}
}
