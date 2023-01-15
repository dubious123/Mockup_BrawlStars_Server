using System;

using static Enums;

namespace Server.Game.GameRule
{
	public abstract class BaseGameRule
	{
		public NetWorld World { get; set; }
		public abstract TeamType GetTeamType(NetObject netObj);
		public abstract bool CanSendHit(NetBaseComponent from, NetBaseComponent to);
		public abstract void Update();
	}
}
