using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
	public abstract class NetCollider2D : NetBaseComponent
	{
		public bool IsTrigger { get; set; } = false;
		public Action<NetCollider2D> OnCollided;

		public abstract bool CheckCollision(NetCollider2D other);

		public NetCollider2D SetIsTrigger(bool isTrigger)
		{
			IsTrigger = isTrigger;
			return this;
		}
	}
}
