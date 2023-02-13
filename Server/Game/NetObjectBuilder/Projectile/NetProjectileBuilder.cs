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
				.SetMaxDistance((sfloat)8.25f);
			return obj;
		}

		public static NetObject CreateProjectile_ShellySuperShell(NetObjectBuilder builder)
		{
			var obj = builder.GetRawObject(NetObjectType.Projectile_Shelly_SuperShell);
			obj.AddComponent<NetCircleCollider2D>()
				.SetOffsetAndRadius(sVector2.zero, (sfloat)0.2f);
			obj.AddComponent<NetProjectile>()
				.SetSpeed((sfloat)16)
				.SetMaxDistance((sfloat)10.5f);
			return obj;
		}

		public static NetObject CreateProjectile_SpikeNeedleGranade(NetObjectBuilder builder)
		{
			var obj = builder.GetRawObject(NetObjectType.Projectile_Spike_NeedleGranade);
			obj.AddComponent<NetCircleCollider2D>()
				.SetOffsetAndRadius(sVector2.zero, (sfloat)0.3f);
			obj.AddComponent<NetProjectile>()
				.SetSpeed((sfloat)14)
				.SetMaxDistance((sfloat)10f);
			return obj;
		}

		public static NetObject CreateProjectile_SpikeNeedleGranade_Needle(NetObjectBuilder builder)
		{
			var obj = builder.GetRawObject(NetObjectType.Projectile_Spike_NeedleGranade_Needle);
			obj.AddComponent<NetCircleCollider2D>()
				.SetOffsetAndRadius(sVector2.zero, (sfloat)0.3f);
			obj.AddComponent<NetProjectile>()
				.SetSpeed((sfloat)16)
				.SetMaxDistance((sfloat)10.5f);
			return obj;
		}

		public static NetObject CreateProjectile_Spike_StickAround(NetObjectBuilder builder)
		{
			var obj = builder.GetRawObject(NetObjectType.Projectile_Spike_StickAround);
			obj.AddComponent<NetCircleCollider2D>()
				.SetOffsetAndRadius(sVector2.zero, (sfloat)2.5f);
			obj.AddComponent<NetProjectile>()
				.SetSpeed((sfloat)16)
				.SetMaxDistance((sfloat)10f);
			return obj;
		}
	}
}
