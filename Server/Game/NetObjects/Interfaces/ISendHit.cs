namespace Server.Game
{
	public interface ISendHit : INetObject
	{
		public void SendHit(ITakeHit target, in HitInfo info);
	}
}
