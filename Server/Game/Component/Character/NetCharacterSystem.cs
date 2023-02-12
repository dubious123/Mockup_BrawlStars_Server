namespace Server.Game
{
	public class NetCharacterSystem : NetBaseComponentSystem<NetCharacter>
	{
		private int _characterCount = 0;

		public override bool AddComponent(NetObjectId netObjectId, NetCharacter component)
		{
			component.SetTeamId(_characterCount++);
			return base.AddComponent(netObjectId, component);
		}

		public override void Update()
		{
			if (Active is false)
			{
				return;
			}

			foreach (var player in ComponentDict)
			{
				if (player.Active)
				{
					(player as INetUpdatable)?.Update();
				}
			}
		}

		public override void Reset()
		{
			base.Reset();
			foreach (var character in ComponentDict)
			{
				character.Position = World.Data.SpawnPoints[character.TeamId];
				character.Rotation = sQuaternion.identity;
				character.Reset();
			}
		}

		public void SetVisible(NetCharacter character, bool visible)
		{
			character.IsVisible = visible;
		}
	}
}
