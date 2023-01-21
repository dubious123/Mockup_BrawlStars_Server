using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Enums;

namespace Server.Game
{
	public class NetEnvSystem : NetBaseComponentSystem<NetEnv>
	{
#if CLIENT
		public Action<NetTree, NetCharacter> OnCharEnterTree;
		public Action<NetTree, NetCharacter> OnCharExitTree;
#endif
		private Dictionary<NetCharacter, HashSet<NetTree>> _steppedTreeDict;

		public override void Init(NetWorld world)
		{
			base.Init(world);
			foreach (var netObjData in World.Data.NetObjectDatas)
			{
				var obj = World.ObjectBuilder.GetNewObject((NetObjectType)netObjData.NetObjectId)
					.SetPositionAndRotation(netObjData.Position, netObjData.Rotation);
				var collider = obj.GetComponent<NetBoxCollider2D>();
				collider?.SetOffsetAndSize(netObjData.BoxCollider.Offset, netObjData.BoxCollider.Size);
			}
		}

		public override void Update()
		{
			if (Active is false)
			{
				return;
			}

			foreach (var c in ComponentDict)
			{
				if (c.Active)
				{
					(c as INetUpdatable)?.Update();
				}
			}

			foreach (var character in World.CharacterSystem.ComponentDict)
			{
				if (_steppedTreeDict[character].Count == 0)
				{
					World.CharacterSystem.SetVisible(character, true);
					continue;
				}

				var otherTeamTree = _steppedTreeDict.Where(pair => pair.Key.Team != character.Team);
				foreach (var tree in _steppedTreeDict[character])
				{
					foreach (var pair in otherTeamTree)
					{
						if (pair.Value.Contains(tree))
						{
							World.CharacterSystem.SetVisible(character, true);
							goto Continue;
						}
					}
				}

				World.CharacterSystem.SetVisible(character, false);
			Continue:
				continue;
			}
		}

		public override void Reset()
		{
			base.Reset();

			if (_steppedTreeDict is null)
			{
				_steppedTreeDict ??= new(World.CharacterSystem.ComponentDict.Count);
				foreach (var character in World.CharacterSystem.ComponentDict)
				{
					_steppedTreeDict.Add(character, new());
				}
			}
			else
			{
				foreach (var treeSet in _steppedTreeDict.Values)
				{
					treeSet.Clear();
				}
			}

			foreach (var env in ComponentDict)
			{
				env.Reset();
			}
		}

		public void OnCharacterEnterTree(NetTree tree, NetCharacter character)
		{
			_steppedTreeDict[character].Add(tree);
#if CLIENT
			OnCharEnterTree?.Invoke(tree, character);
#endif
		}

		public void OnCharacterExitTree(NetTree tree, NetCharacter character)
		{
			_steppedTreeDict[character].Remove(tree);
#if CLIENT
			OnCharExitTree?.Invoke(tree, character);
#endif
		}
	}
}
