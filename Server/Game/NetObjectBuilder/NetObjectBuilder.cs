namespace Server.Game
{
	public class NetObjectBuilder
	{
		public NetWorld World { get; init; }

		private static readonly Dictionary<NetObjectType, Func<NetObjectBuilder, NetObject>> _builderDict;
		private readonly uint[] _instanceNumDict;

		static NetObjectBuilder()
		{
			_builderDict = new();
			_builderDict.TryAdd(NetObjectType.Env_Wall, NetEnvBuilder.CreateWall);
			_builderDict.TryAdd(NetObjectType.Env_Tree, NetEnvBuilder.CreateTree);
			_builderDict.TryAdd(NetObjectType.Character_Shelly, NetCharacterBuilder.CreateShelly);
			_builderDict.TryAdd(NetObjectType.Character_Spike, NetCharacterBuilder.CreateSpike);
			_builderDict.TryAdd(NetObjectType.Projectile_Shelly_Buckshot, NetProjectileBuilder.CreateProjectile_ShellyBuckshot);
			_builderDict.TryAdd(NetObjectType.Projectile_Shelly_SuperShell, NetProjectileBuilder.CreateProjectile_ShellySuperShell);
			_builderDict.TryAdd(NetObjectType.Projectile_Spike_NeedleGranade, NetProjectileBuilder.CreateProjectile_SpikeNeedleGranade);
			_builderDict.TryAdd(NetObjectType.Projectile_Spike_NeedleGranade_Needle, NetProjectileBuilder.CreateProjectile_SpikeNeedleGranade_Needle);
			_builderDict.TryAdd(NetObjectType.Projectile_Spike_StickAround, NetProjectileBuilder.CreateProjectile_Spike_StickAround);
			_builderDict.TryAdd(NetObjectType.Projectile_Spike_StickAround_Aoe, NetProjectileBuilder.CreateProjectile_Spike_StickAround_Aoe);

		}

		public NetObjectBuilder()
		{
			var tags = (NetObjectType[])Enum.GetValues(typeof(NetObjectType));
			_instanceNumDict = new uint[tags.Length];
		}

		public NetObject GetNewObject(NetObjectType type)
		{
			var obj = _builderDict[type].Invoke(this);
			World.AddNewNetObject(obj);
			return obj;
		}

		public NetObject GetRawObject(NetObjectType type)
		{
			var instanceId = _instanceNumDict[(int)type]++;
			return instanceId > 0xffff
				? throw new Exception("too many instances")
				: new NetObject()
				{
					World = World,
					ObjectId = NetObjectId.FromRaw((((uint)type) << 16) | instanceId),
				};
		}

		public void Reset()
		{
			for (int i = _instanceNumDict.Length - 1; i >= 0; --i)
			{
				_instanceNumDict[i] = 0;
			}
		}
	}
}
