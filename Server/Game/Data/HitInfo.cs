namespace Server.Game
{
	public class HitInfo
	{
		public int Damage { get; init; }
		public sfloat KnockbackDistance { get; init; }
		public int KnockbackDuration { get; init; }
		public int StunDuration { get; init; }
	}
}