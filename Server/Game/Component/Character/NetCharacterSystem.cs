using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
	public class NetCharacterSystem : NetBaseComponentSystem<NetCharacter>
	{
		public override void Reset()
		{
			foreach (var character in ComponentDict)
			{
				character.Position = World.Data.SpawnPoints[character.NetObjId.InstanceId];
				character.Rotation = sQuaternion.identity;
				character.Reset();
			}
		}
	}
}
