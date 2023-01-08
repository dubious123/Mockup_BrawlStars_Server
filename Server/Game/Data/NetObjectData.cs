using System;

namespace Server.Game.Data
{
	public class NetObjectData
	{
		public uint NetObjectId { get; init; }
		public sVector3 Position { get; init; }
		public sQuaternion Rotation { get; init; }
		public NetBoxCollider2DData BoxCollider { get; init; }
		public NetCircleColllider2DData CircleCollider { get; init; }
	}
}
