
using System.Collections.Generic;

using static Enums;

namespace Server.Game
{
	public class Wall : INetCollidable2D
	{
		public NetWorld World { get; init; }
		public sVector3 Position { get; set; }
		public sQuaternion Rotation { get; set; }
		public NetObjectTag Tag { get; set; }
		public INetCollider2D Collider { get; init; }

		public Wall(NetWorld world, NetObjectTag tag, sVector2 colliderOffset, sVector2 colliderSize)
		{
			World = world;
			Tag = tag;
			Collider = new NetBoxCollider2D(this, colliderOffset, colliderSize);
		}

		public virtual bool CheckCollision(NetObjectTag tag)
		{
			return World.Physics2D.DetectCollision(Collider, tag);
		}

		public virtual void GetCollisions(NetObjectTag tag, List<INetCollider2D> collisions)
		{
			World.Physics2D.GetCollisions(Collider, tag, collisions);
		}
	}
}
