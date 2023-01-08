using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Enums;

namespace Server.Game
{
	public abstract class NetBaseComponent
	{
		public NetObject NetObj { get; init; }
		public NetObjectId NetObjId => NetObj.ObjectId;
		public NetWorld World => NetObj.World;
		public NetObjectBuilder ObjectBuilder => NetObj.World.ObjectBuilder;
		public sVector3 Position { get => NetObj.Position; set => NetObj.Position = value; }
		public sQuaternion Rotation { get => NetObj.Rotation; set => NetObj.Rotation = value; }

		public bool Active { get; set; } = true;

		public virtual void Start() { }
	}
}
