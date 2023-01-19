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
		private Dictionary<NetCharacter, int> _steppedTreeCountDict;

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

		public override void Reset()
		{
			base.Reset();

			if (_steppedTreeCountDict is null)
			{
				_steppedTreeCountDict ??= new(World.CharacterSystem.ComponentDict.Count);
				foreach (var character in World.CharacterSystem.ComponentDict)
				{
					_steppedTreeCountDict.Add(character, 0);
				}
			}
			else
			{
				_steppedTreeCountDict.ResetValues(0);
			}

			foreach (var env in ComponentDict)
			{
				env.Reset();
			}
		}

		public void OnCharacterEnterTree(NetTree tree, NetCharacter character)
		{
			if (_steppedTreeCountDict[character] == 0)
			{
				World.CharacterSystem.SetVisible(character, false);
			}

			_steppedTreeCountDict[character]++;
#if CLIENT
			OnCharEnterTree?.Invoke(tree, character);
#endif
		}

		public void OnCharacterExitTree(NetTree tree, NetCharacter character)
		{
			_steppedTreeCountDict[character]--;
			if (_steppedTreeCountDict[character] == 0)
			{
				World.CharacterSystem.SetVisible(character, true);
			}
#if CLIENT
			OnCharExitTree?.Invoke(tree, character);
#endif
		}
	}
}
