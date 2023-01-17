using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
	public class NetProjectileSystem : NetBaseComponentSystem<NetProjectile>
	{
		public override void Reset()
		{
			base.Reset();
			foreach (NetProjectile p in ComponentDict)
			{
				p.Reset();
			}
		}
	}
}
