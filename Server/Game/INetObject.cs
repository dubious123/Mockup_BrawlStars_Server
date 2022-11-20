
using static Enums;

namespace Server.Game
{
	public interface INetObject
	{
		public uint ObjectId { get; init; }
		public sVector3 Position { get; set; }
		public sQuaternion Rotation { get; set; }
		public NetObjectTag Tag { get; set; }
		public NetWorld World { get; init; }
	}
}

