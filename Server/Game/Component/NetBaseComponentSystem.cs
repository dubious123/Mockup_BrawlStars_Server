using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Enums;

namespace Server.Game
{
	public abstract class NetBaseComponentSystem<T> : INetUpdatable where T : NetBaseComponent
	{
		public NetWorld World { get; init; }

		public DetDictionary<NetObjectId, T> ComponentDict => _componentDict;

		private DetDictionary<NetObjectId, T> _componentDict = new();

		public bool AddComponent(NetObjectId netObjectId, T component)
		{
			return _componentDict.TryAdd(netObjectId, component);
		}

		public bool AddComponent(NetObject netObject, T component)
			=> AddComponent(netObject.ObjectId, component);

		public T GetComponent(NetObjectId netObjectId)
		{
			_componentDict.TryGetValue(netObjectId, out var component);
			return component;
		}

		public T GetComponent(NetObject netOjbect)
		{
			_componentDict.TryGetValue(netOjbect.ObjectId, out var component);
			return component;
		}

		public virtual void Update()
		{
			foreach (var c in _componentDict)
			{
				if (c.Active)
				{
					(c as INetUpdatable)?.Update();
				}
			}
		}
	}
}
