

using System.Collections.Generic;
using System.Linq;

using static Enums;

namespace Server.Game
{
	public class NetObject
	{
		public NetObjectId ObjectId { get; init; }
		public sVector3 Position { get; set; }
		public sQuaternion Rotation { get; set; }
		public NetObjectType Tag => ObjectId.Type;
		public NetWorld World { get; init; }

		public NetObject SetPositionAndRotation(sVector3 position, sQuaternion rotation)
		{
			Position = position;
			Rotation = rotation;
			return this;
		}

		public override int GetHashCode()
		{
			return (int)ObjectId.GetRaw();
		}
	}
}
