using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
	public class NetCharacterSystem : NetBaseComponentSystem<NetCharacter>
	{
		public override void Update()
		{
			if (Active is false)
			{
				return;
			}

			foreach (var player in ComponentDict)
			{
				if (player.Active)
				{
					(player as INetUpdatable)?.Update();
					Loggers.Game.Information("Player [{0}]", player.NetObj.ObjectId.InstanceId);
					Loggers.Game.Information("Position [{0:x},{1:x},{2:x}]] : ", player.Position.x.RawValue, player.Position.y.RawValue, player.Position.z.RawValue);
				}
			}
		}

		public override void Reset()
		{
			base.Reset();
			foreach (var character in ComponentDict)
			{
				character.Position = World.Data.SpawnPoints[character.NetObjId.InstanceId];
				character.Rotation = sQuaternion.identity;
				character.Reset();
			}
		}

		public void SetVisible(NetCharacter character, bool visible)
		{
			character.IsVisible = visible;
		}
	}
}
