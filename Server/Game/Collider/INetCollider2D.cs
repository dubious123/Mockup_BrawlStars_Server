using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
	public interface INetCollider2D
	{
		public INetObject NetObject { get; init; }

		public bool CheckCollision(INetCollider2D other);
	}
}
