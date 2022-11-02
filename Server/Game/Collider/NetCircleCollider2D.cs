
namespace Server.Game
{
	public class NetCircleCollider2D : INetCollider2D
	{
		public INetObject NetObject { get; init; }
		public sVector3 Offset { get; init; }
		public sVector3 Center => NetObject.Position + Offset;
		public sfloat Radius { get; init; }
		public sfloat RadiusSquared { get; init; }

		public NetCircleCollider2D(INetObject netObject, sVector2 offset, sfloat radius)
		{
			NetObject = netObject;
			Radius = radius;
			RadiusSquared = radius * radius;
			Offset = offset;
		}

		public bool CheckCollision(INetCollider2D other)
		{
			return other switch
			{
				NetBoxCollider2D box => NetPhysics2D.CheckBoxCircleCollision(box, this),
				NetCircleCollider2D circle => NetPhysics2D.CheckCircleCircleCollision(this, circle),
				_ => false
			};
		}
	}
}
