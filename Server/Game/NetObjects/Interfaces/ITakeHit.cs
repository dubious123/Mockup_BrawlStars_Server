namespace Server.Game
{
	public interface ITakeHit : INetObject
	{
		public bool CanBeHit();
		public void TakeMeleeDamage(int damage);
		public void TakeKnockback(int duration, sVector3 delta);
		public void TakeStun(int duration);
	}
}
