using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Enums;

namespace Server.Game
{
	public class NetCollider2DSystem : NetBaseComponentSystem<NetCollider2D>
	{
		private List<NetCollider2D> _listeners = new();
		private List<NetCollider2D> _senders = new();

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
			var deltaY = left.Center.z - right.Center.z;
			var dist = left.Radius + right.Radius;
			return deltaX * deltaX + deltaY * deltaY <= dist * dist;
		}

		public bool DetectCollision(NetCollider2D collider)
		{
			foreach (var c in ComponentDict)
			{
				if (c != collider && collider.Active is true && collider.CheckCollision(c))
				{
					return true;
				}
			}

			return false;
		}

		public bool DetectCollision(NetCollider2D collider, NetObjectType tag)
		{
			foreach (var c in ComponentDict)
			{
				if (c.NetObj.Tag == tag && c != collider && collider.Active is true && collider.CheckCollision(c))
				{
					return true;
				}
			}

			return false;
		}

		public void GetCollisions(NetCollider2D collider, IList<NetCollider2D> collisions)
		{
			foreach (var c in ComponentDict)
			{
				if (c != collider && collider.CheckCollision(c))
				{
					collisions.Add(c);
				}
			}
		}

		public void GetCollisions(NetCollider2D collider, NetObjectType tag, IList<NetCollider2D> collisions)
		{
			foreach (var c in ComponentDict)
			{
				if (c.NetObj.Tag == tag && c != collider && collider.CheckCollision(c))
				{
					collisions.Add(c);
				}
			}
		}

		public override void Update()
		{
			if (Active is false)
			{
				return;
			}

			_listeners.Clear();
			_senders.Clear();
			var activeComponents = ComponentDict
				.Where(c => c.Active)
				.ToArray();

			foreach (var colliders in activeComponents)
			{
				(colliders as INetUpdatable)?.Update();
				if (colliders.IsTrigger)
				{
					_listeners.Add(colliders);
					continue;
				}

				_senders.Add(colliders);
			}

			for (int i = _senders.Count() - 1; 0 <= i; --i)
			{
				var sender = _senders[i];
				for (int j = i - 1; 0 <= j; --j)
				{
					var to = _senders[j];
					if (sender.CheckCollision(to) is false)
					{
						continue;
					}

					sender.OnCollided?.Invoke(to);
					to.OnCollided?.Invoke(sender);
				}

				foreach (var listener in _listeners)
				{
					if (sender.CheckCollision(listener) is false)
					{
						continue;
					}

					sender.OnCollided?.Invoke(listener);
					listener.OnCollided?.Invoke(sender);
				}
			}
		}
	}
}
