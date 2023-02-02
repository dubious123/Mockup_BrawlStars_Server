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
				.SetSpeed((sfloat)14)
				.SetMaxTravelTime(35);
			return obj;
		}

		public static NetObject CreateProjectile_ShellySuperShell(NetObjectBuilder builder)
		{
			var obj = builder.GetRawObject(NetObjectType.Projectile_Shelly_SuperShell);
			obj.AddComponent<NetCircleCollider2D>()
				.SetOffsetAndRadius(sVector2.zero, (sfloat)0.2f);
			obj.AddComponent<NetProjectile>()
				.SetSpeed((sfloat)16)
				.SetMaxTravelTime(40);
			return obj;
		}
	}
}
