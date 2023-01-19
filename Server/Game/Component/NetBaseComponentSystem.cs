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
		public bool Active { get; set; } = true;
		public NetWorld World { get; private set; }
		public DetDictionary<NetObjectId, T> ComponentDict => _componentDict;

		private DetDictionary<NetObjectId, T> _componentDict = new();

		public virtual void Init(NetWorld world)
		{
			World = world;
		}

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

		public void RemoveComponent(NetObject netObj)
		{
			_componentDict.Remove(netObj.ObjectId);
		}

		public void SetActive(NetObject netObj, bool active)
		{
			var component = GetComponent(netObj.ObjectId);
			if (component is not null)
			{
				component.Active = active;
			}
		}

		public void SetActiveAll(bool active)
		{
			foreach (var component in _componentDict)
			{
				component.Active = active;
			}
		}

		public virtual void Update()
		{
			if (Active is false)
			{
				return;
			}

			foreach (var c in _componentDict)
			{
				if (c.Active)
				{
					(c as INetUpdatable)?.Update();
				}
			}
		}

		public virtual void Reset()
		{
			Active = true;
		}
	}
}
