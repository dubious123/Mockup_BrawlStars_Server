using System;

using static Enums;

namespace Server.Game.GameRule
{
	public abstract class BaseGameRule
	{
		public NetWorld World { get; set; }
		public bool Active { get; set; } = true;
		public int CurrentRoundFrameCount { get; protected set; }
		public int MaxFrameCount { get; protected set; }

		public abstract TeamType GetTeamType(NetObject netObj);
		public abstract bool CanSendHit(NetBaseComponent from, NetBaseComponent to);
		public abstract void Update();
		public abstract void Reset();
		public abstract void OnCharacterDead(NetCharacter character);
	}
}
