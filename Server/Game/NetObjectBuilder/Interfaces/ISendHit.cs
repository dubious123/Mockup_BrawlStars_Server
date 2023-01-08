namespace Server.Game
{
	public interface ISendHit
	{
		public void SendHit(ITakeHit target, in HitInfo info);
	}
}
