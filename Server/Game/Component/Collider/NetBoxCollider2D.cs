
using System.Drawing;

namespace Server.Game
{
	public class NetBoxCollider2D : NetCollider2D
	{
		private sfloat _deltaX;
		private sfloat _deltaY;

		public sfloat MaxX => NetObj.Position.x + _deltaX;
		public sfloat MinX => NetObj.Position.x - _deltaX;
		public sfloat MaxY => NetObj.Position.z + _deltaY;
		public sfloat MinY => NetObj.Position.z - _deltaY;

		public NetBoxCollider2D SetOffsetAndSize(sVector2 offset, sVector2 size)
		{
			_deltaX = offset.x + size.x * 0.5f;
			_deltaY = offset.y + size.y * 0.5f;
			return this;
		}

		public override bool CheckCollision(NetCollider2D other)
		{
			return other switch
			{
				NetBoxCollider2D box => NetCollider2DSystem.CheckBoxBoxCollision(this, box),
				NetCircleCollider2D circle => NetCollider2DSystem.CheckBoxCircleCollision(this, circle),
				_ => false
			};
		}
	}
}
