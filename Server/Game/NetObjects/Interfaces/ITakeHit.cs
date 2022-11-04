namespace Server.Game
{
	public interface ITakeHit
	{
		public bool CanBeHit();
		public void TakeMeleeDamage(int damage);
	}
}
