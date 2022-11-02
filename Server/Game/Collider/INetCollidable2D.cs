
using System.Collections.Generic;

using static Enums;

namespace Server.Game
{
	public interface INetCollidable2D : INetObject
	{
		public INetCollider2D Collider { get; init; }
		public bool CheckCollision(NetObjectTag tag);
		public void GetCollisions(NetObjectTag tag, List<INetCollider2D> collisions);
	}
}
