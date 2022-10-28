namespace Server.Game
{
	public record HitInfo
	{
		public int Damage { get; init; }
		public StunInfo StunInfo { get; init; }
		public KnockbackInfo KnockbackInfo { get; init; }
	}

	public record KnockbackInfo
	{
		public Vector3 Direction { get; set; }
		public sfloat Duration { get; init; }
		public sfloat Speed { get; init; }
	}

	public record StunInfo
	{
		public sfloat Duration { get; init; }
	}
}
