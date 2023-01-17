using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
	public class NetEnvSystem : NetBaseComponentSystem<NetEnv>
	{
		public override void Reset()
		{
			base.Reset();
			var list = ComponentDict.AsEnumerable().ToArray();
			foreach (var env in list)
			{
				env.NetObj.Destroy();
			}

			int i = 0;
			foreach (var netObjData in World.Data.NetObjectDatas)
			{
				i++;
				if (i == 27)
				{
					Loggers.Debug.Debug("hi");
				}
				var obj = World.ObjectBuilder.GetNewObject(NetObjectType.Env_Wall)
					.SetPositionAndRotation(netObjData.Position, netObjData.Rotation);
				var collider = obj.GetComponent<NetBoxCollider2D>();
				collider.SetOffsetAndSize(netObjData.BoxCollider.Offset, netObjData.BoxCollider.Size);
			}
		}
	}
}
