namespace Server.Game
{
	public class NetProjectileSystem : NetBaseComponentSystem<NetProjectile>
	{
		public readonly HashSet<NetProjectile> ActiveSet = new(120);
		private readonly Stack<NetProjectile>[] _reservePool = new Stack<NetProjectile>[(int)NetObjectType.Projectile_Spike_StickAround - (int)NetObjectType.Projectile_Shelly_Buckshot + 1] { new(90), new(30), new(3 * Config.MAX_PLAYER_COUNT), new(18 * Config.MAX_PLAYER_COUNT), new(Config.MAX_PLAYER_COUNT) };
		private readonly List<NetProjectile> _removeList = new(120);

		public static int GetIndex(NetObjectType type)
		{
			return type - NetObjectType.Projectile_Shelly_Buckshot;
		}

		public void Reserve(NetObjectType type, int count)
		{
			for (int i = 0; i < count; ++i)
			{
				var obj = World.ObjectBuilder.GetNewObject(type);
				obj.Active = false;
				_reservePool[GetIndex(type)].Push(obj.GetComponent<NetProjectile>());
			}
		}

		public void Awake(NetObjectType type, Action<NetProjectile> initFunc = null)
		{
			var projectile = _reservePool[GetIndex(type)].Pop();
			initFunc?.Invoke(projectile);
			ActiveSet.Add(projectile);
		}

		public void Awake(NetObjectType type, int count, Action<int, NetProjectile> initFunc = null)
		{
			Reserve(type, count - _reservePool[GetIndex(type)].Count);

			for (int i = 0; i < count; ++i)
			{
				var projectile = _reservePool[GetIndex(type)].Pop();
				initFunc?.Invoke(i, projectile);
				ActiveSet.Add(projectile);
			}
		}

		public void Return(NetProjectile projectile, Action<NetProjectile> resetFunc = null)
		{
			resetFunc?.Invoke(projectile);
			_removeList.Add(projectile);
		}

		public override void Update()
		{
			if (Active is false)
			{
				return;
			}

			foreach (var p in ActiveSet)
			{
				p.Update();
			}

			RemoveInternal();
		}

		public override void Reset()
		{
			base.Reset();
			foreach (NetProjectile p in ComponentDict)
			{
				p.Reset();
			}
		}

		private void RemoveInternal()
		{
			foreach (var projectile in _removeList)
			{
				ActiveSet.Remove(projectile);
				_reservePool[GetIndex(projectile.NetObjId.Type)].Push(projectile);
			}

			_removeList.Clear();
		}
	}
}
