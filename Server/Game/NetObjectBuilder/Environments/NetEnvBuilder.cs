using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Enums;

namespace Server.Game
{
	public class NetEnvBuilder
	{
		public static NetObject CreateWall(NetObjectBuilder builder)
		{
			var obj = builder.GetRawObject(NetObjectType.Env_Wall);
			obj.AddComponent<NetBoxCollider2D>()
				.SetIsTrigger(true);
			obj.AddComponent<NetWall>();
			return obj;
		}

		public static NetObject CreateTree(NetObjectBuilder builder)
		{
			var obj = builder.GetRawObject(NetObjectType.Env_Tree);
			obj.AddComponent<NetBoxCollider2D>()
				.SetOffsetAndSize(sVector2.zero, sVector2.one)
				.SetIsTrigger(true);
			obj.AddComponent<NetTree>();
			return obj;
		}
	}
}
