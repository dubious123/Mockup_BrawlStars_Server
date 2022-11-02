
using System;
using System.Collections.Generic;

using static Enums;

namespace Server.Game
{
	public class NetPhysics2D
	{
		private List<INetCollider2D>[] _colliders;

		public NetPhysics2D()
		{
			var length = Enum.GetValues(typeof(NetObjectTag)).Length;
			_colliders = new List<INetCollider2D>[length];
			for (int i = 0; i < length; i++)
			{
				_colliders[i] = new();
			}
		}

		public static bool CheckBoxBoxCollision(NetBoxCollider2D left, NetBoxCollider2D right)
		{
			return
				left.MinX <= right.MaxX &&
				left.MaxX >= right.MinX &&
				left.MinY <= right.MaxY &&
				left.MaxY >= right.MinY;
		}

		public static bool CheckBoxCircleCollision(NetBoxCollider2D box, NetCircleCollider2D circle)
		{
			var x = sMathf.Max(box.MinX, sMathf.Min(circle.Center.x, box.MaxX));
			var y = sMathf.Max(box.MinY, sMathf.Min(circle.Center.z, box.MaxY));
			return
				(x - circle.Center.x) * (x - circle.Center.x) +
				(y - circle.Center.z) * (y - circle.Center.z) <= circle.RadiusSquared;
		}

		public static bool CheckCircleCircleCollision(NetCircleCollider2D left, NetCircleCollider2D right)
		{
			var deltaX = left.Center.x - right.Center.x;
			var deltaY = left.Center.y - right.Center.y;
			var dist = left.Radius + right.Radius;
			return deltaX * deltaX + deltaY * deltaY <= dist * dist;
		}

		public void RegisterCollider(INetCollider2D collider)
		{
			_colliders[(int)collider.NetObject.Tag].Add(collider);
		}

		public bool DetectCollision(INetCollider2D collider, NetObjectTag tag)
		{
			foreach (var c in _colliders[(int)tag])
			{
				if (c != collider && collider.CheckCollision(c))
				{
					return true;
				}
			}

			return false;
		}

		public void GetCollisions(INetCollider2D collider, NetObjectTag tag, IList<INetCollider2D> collisions)
		{
			foreach (var c in _colliders[(int)tag])
			{
				if (c == collider)
				{
					continue;
				}

				if (collider.CheckCollision(c))
				{
					collisions.Add(c);
				}
			}
		}
	}
}
