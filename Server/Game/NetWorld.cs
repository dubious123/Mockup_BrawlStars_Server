using Server.Game.Data;
using Server.Logs;

using static Enums;

namespace Server.Game
{
	public class NetWorld
	{
		private Action _update;
		private Dictionary<uint, INetObject> _netObjDict = new();
		public NetPhysics2D Physics2D = new();
		public GameFrameInfo InputInfo { get; set; }
		public NetCharacter[] NetCharacters { get; set; }

		public NetWorld(WorldData data)
		{
			_update = UpdateGameLogic;
			_update += UpdatePlayers;
			uint i = 0x10;
			foreach (var netObjData in data.NetObjectDatas)
			{
				var wall = new Wall(this, NetObjectTag.Wall, netObjData.BoxCollider.Offset, netObjData.BoxCollider.Size)
				{
					Position = netObjData.Position,
					Rotation = netObjData.Rotation,
				};

				AddNewNetObject(i++, wall);
			}

			NetCharacters = new NetCharacter[6];
		}

		public void Update() => _update();

		public void UpdateGameLogic()
		{

		}

		public void UpdatePlayers()
		{
			for (int i = 0; i < 6; i++)
			{
				var player = NetCharacters[i];
				if (player is null)
				{
					return;
				}

				Loggers.Game.Information("Player [{0}]", i);
				player.UpdateInput(InputInfo.Inputs[i]);
				player.Update();
				Loggers.Game.Information("Position [{0:x},{1:x},{2:x}]] : ", player.Position.x.RawValue, player.Position.y.RawValue, player.Position.z.RawValue);
			}
		}


		public void AddNewNetObject(uint inGameId, INetObject obj)
		{
			if (obj is NetCharacter)
			{
				NetCharacters[inGameId] = (obj as NetCharacter);
			}
			else if (obj is INetUpdatable)
			{
				_update += (obj as INetUpdatable).Update;
			}

			if (obj is INetCollidable2D)
			{
				Physics2D.RegisterCollider((obj as INetCollidable2D).Collider);
			}

			_netObjDict.Add(inGameId, obj);
		}

		public INetObject FindNetObject(uint inGameId) => _netObjDict[inGameId];

		public void FindNetObjects(Func<INetObject, bool> condition, IList<INetObject> result)
		{
			foreach (var obj in _netObjDict.Values)
			{
				if (condition(obj))
				{
					result.Add(obj);
				}
			}
		}

		public bool FindAllAndBroadcast(Func<INetObject, bool> condition, Action<INetObject> action)
		{
			var count = 0;
			foreach (var obj in _netObjDict.Values)
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

