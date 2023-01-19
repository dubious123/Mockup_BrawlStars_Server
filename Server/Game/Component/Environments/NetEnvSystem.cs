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
		private Dictionary<NetCharacter, int> _steppedTreeCountDict;
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


			var list = ComponentDict.AsEnumerable().ToArray();
			foreach (var env in list)
			{
				env.NetObj.Destroy();
			}

			foreach (var netObjData in World.Data.NetObjectDatas)
			{
				var obj = World.ObjectBuilder.GetNewObject((NetObjectType)netObjData.NetObjectId)
					.SetPositionAndRotation(netObjData.Position, netObjData.Rotation);
				var collider = obj.GetComponent<NetBoxCollider2D>();
				collider?.SetOffsetAndSize(netObjData.BoxCollider.Offset, netObjData.BoxCollider.Size);
			}
		}

		public void OnCharacterEnterTree(NetTree tree, NetCharacter character)
		{
			if (_steppedTreeCountDict[character] == 0)
			{
				World.CharacterSystem.SetVisible(character, false);
			}

			_steppedTreeCountDict[character]++;

		}

		public void OnCharacterExitTree(NetTree tree, NetCharacter character)
		{
			_steppedTreeCountDict[character]--;
			if (_steppedTreeCountDict[character] == 0)
			{
				World.CharacterSystem.SetVisible(character, true);
			}
		}
	}
}
