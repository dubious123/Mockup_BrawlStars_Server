
using System.Drawing;

namespace Server.Game
{
	public class NetBoxCollider2D : INetCollider2D
	{
		private readonly sfloat _deltaX;
		private readonly sfloat _deltaY;

		public INetObject NetObject { get; init; }

		public sfloat MaxX => NetObject.Position.x + _deltaX;
		public sfloat MinX => NetObject.Position.x - _deltaX;
		public sfloat MaxY => NetObject.Position.z + _deltaY;
		public sfloat MinY => NetObject.Position.z - _deltaY;

		public NetBoxCollider2D(INetObject netObject, sVector2 offset, sVector2 size)
		{
			NetObject = netObject;
			_deltaX = offset.x + size.x * 0.5f;
			_deltaY = offset.y + size.y * 0.5f;
		}

		public bool CheckCollision(INetCollider2D other)
		{
			return other switch
			{
				NetBoxCollider2D box => NetPhysics2D.CheckBoxBoxCollision(this, box),
				NetCircleCollider2D circle => NetPhysics2D.CheckBoxCircleCollision(this, circle),
				_ => false
			};
		}
	}
}
