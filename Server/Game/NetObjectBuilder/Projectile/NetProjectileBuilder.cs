using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Enums;

namespace Server.Game
{
	public class NetProjectileBuilder
	{
		public static NetObject CreateProjectile_ShellyBuckshot(NetObjectBuilder builder)
		{
			var obj = builder.GetRawObject(NetObjectType.Projectile_Shelly_Buckshot);
			obj.AddComponent<NetCircleCollider2D>()
				.SetOffsetAndRadius(sVector2.zero, (sfloat)0.2f);
			obj.AddComponent<NetProjectile>()
				.SetSpeed((sfloat)10 / 48 * 60)
				.SetMaxTravelTime(48);
			return obj;
		}

		public static NetObject CreateProjectile_ShellySuperShell(NetObjectBuilder builder)
		{
			var obj = builder.GetRawObject(NetObjectType.Projectile_Shelly_SuperShell);
			obj.AddComponent<NetCircleCollider2D>()
				.SetOffsetAndRadius(sVector2.zero, (sfloat)0.2f);
			obj.AddComponent<NetProjectile>()
				.SetSpeed((sfloat)12)
				.SetMaxTravelTime(60);
			return obj;
		}
	}
}
