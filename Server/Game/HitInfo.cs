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
		public float Duration { get; init; }
		public float Speed { get; init; }
	}

	public record StunInfo
	{
		public float Duration { get; init; }
	}
}
