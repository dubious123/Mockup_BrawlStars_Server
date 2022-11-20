namespace Server.Game.GameRule
{
	public interface IGameRule
	{
		public TeamType GetTeamType(uint objectId);
		public bool CanSendHit(INetObject from, INetObject to);
	}
}
