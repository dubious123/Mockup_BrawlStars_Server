using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Server.Game.Data;
using Server.Game.GameRule;

using static Enums;

namespace Server.Game
{
	public class NetWorld
	{
		public readonly BaseGameRule GameRule;

		public Dictionary<NetObjectId, NetObject> NetObjectDict { get; private set; } = new();
		public WorldData Data => _worldData;
		public NTiming NetTiming = new();
		public NetObjectBuilder ObjectBuilder { get; }
		public NetCollider2DSystem ColliderSystem { get; } = new();
		public NetCharacterSystem CharacterSystem { get; } = new();
		public NetEnvSystem EnvSystem { get; } = new();
		public NetProjectileSystem ProjectileSystem { get; } = new();
		public NetCharacter[] NetCharacters = new NetCharacter[6];
		public bool Active { get; set; } = true;

		private readonly WorldData _worldData;

		public NetWorld(WorldData data, BaseGameRule gameRule)
		{
			_worldData = data;
			GameRule = gameRule;
			GameRule.World = this;
			ObjectBuilder = new() { World = this };
			ColliderSystem.Init(this);
			CharacterSystem.Init(this);
			EnvSystem.Init(this);
			ProjectileSystem.Init(this);
		}

		public void SetNetObjectActive(NetObject netObj, bool active)
		{
			ColliderSystem.SetActive(netObj, active);
			CharacterSystem.SetActive(netObj, active);
			EnvSystem.SetActive(netObj, active);
			ProjectileSystem.SetActive(netObj, active);
		}

		public void Clear()
		{
			ProjectileSystem.Reset();
			CharacterSystem.SetActiveAll(false);
		}

		public void Reset()
		{
			Active = true;
			GameRule.Reset();
			ColliderSystem.Reset();
			CharacterSystem.Reset();
			EnvSystem.Reset();
			ProjectileSystem.Reset();
		}

		public void UpdateInputs(GameFrameInfo inputInfo)
		{
			foreach (var player in CharacterSystem.ComponentDict)
			{
				player.UpdateInput(inputInfo.Inputs[player.NetObj.ObjectId.InstanceId]);
			}
		}

		public void Update()
		{
			if (Active is false)
			{
				return;
			}

			GameRule.Update();
			NetTiming.Update();
			ColliderSystem.Update();
			CharacterSystem.Update();
			EnvSystem.Update();
			ProjectileSystem.Update();
		}

		public void AddNewNetObject(NetObject obj)
		{
			NetObjectDict.Add(obj.ObjectId, obj);
		}

		public void RemoveNetObject(NetObject obj)
		{
			NetObjectDict.Remove(obj.ObjectId);
			ColliderSystem.RemoveComponent(obj);
			CharacterSystem.RemoveComponent(obj);
			EnvSystem.RemoveComponent(obj);
			ProjectileSystem.RemoveComponent(obj);
		}

		public NetObject FindNetObject(NetObjectId inGameId) => NetObjectDict[inGameId];

		public void FindNetObjects(Func<NetObject, bool> condition, IList<NetObject> result)
		{
			foreach (var obj in NetObjectDict.Values)
			{
				if (condition(obj))
				{
					result.Add(obj);
				}
			}
		}

		public bool FindAllAndBroadcast(Func<NetObject, bool> condition, Action<NetObject> action)
		{
			var count = 0;
			foreach (var obj in NetObjectDict.Values)
			{
				if (condition(obj))
				{
					count++;
					action(obj);
				}
			}

			return count > 0;
		}
	}
}

