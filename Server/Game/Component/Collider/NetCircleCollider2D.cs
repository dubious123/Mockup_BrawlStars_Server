
namespace Server.Game
{
	public class NetCircleCollider2D : NetCollider2D
	{
		public sVector3 Offset { get; private set; }
		public sVector3 Center => NetObj.Position + Offset;
		public sfloat Radius { get; private set; }
		public sfloat RadiusSquared { get; private set; }

		public NetCircleCollider2D SetOffsetAndRadius(sVector2 offset, sfloat radius)
		{
			Radius = radius;
			RadiusSquared = radius * radius;
			Offset = offset;
			return this;
		}

		public override bool CheckCollision(NetCollider2D other)
		{
			return other switch
			{
				NetBoxCollider2D box => NetCollider2DSystem.CheckBoxCircleCollision(box, this),
				NetCircleCollider2D circle => NetCollider2DSystem.CheckCircleCircleCollision(this, circle),
				_ => false
			};
		}
	}
}
