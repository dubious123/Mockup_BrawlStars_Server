using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Enums;

namespace Server.Game
{
	public class NetObjectBuilder
	{
		public NetWorld World { get; init; }

		private static readonly ConcurrentDictionary<NetObjectType, Func<NetObjectBuilder, NetObject>> _builderDict;
		private readonly uint[] _instanceNumDict;

		static NetObjectBuilder()
		{
			_builderDict = new();
			_builderDict.TryAdd(NetObjectType.Env_Wall, (builder) => NetEnvBuilder.CreateWall(builder));
			_builderDict.TryAdd(NetObjectType.Env_Tree, (builder) => NetEnvBuilder.CreateTree(builder));
			_builderDict.TryAdd(NetObjectType.Character_Shelly, (builder) => NetCharacterBuilder.CreateShelly(builder));
			_builderDict.TryAdd(NetObjectType.Projectile_Shelly_Buckshot, (builder) => NetProjectileBuilder.CreateProjectile_ShellyBuckshot(builder));
			_builderDict.TryAdd(NetObjectType.Projectile_Shelly_SuperShell, (builder) => NetProjectileBuilder.CreateProjectile_ShellySuperShell(builder));
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
			if (instanceId > 0xffff)
			{
				throw new Exception("too many instances");
			}

			return new NetObject()
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
