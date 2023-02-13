public static partial class Enums
{
	public enum NetObjectType
	{
		None,

		#region Env
		EnvStart,
		Env_Wall,
		Env_Tree,
		EnvEnd,
		#endregion

		#region Character
		CharacterStart,
		Character_Shelly,
		Character_Spike,
		CharacterEnd,
		#endregion

		#region Projectile
		ProjectileStart,
		Projectile_Shelly_Buckshot,
		Projectile_Shelly_SuperShell,
		Projectile_Spike_NeedleGranade,
		Projectile_Spike_NeedleGranade_Needle,
		Projectile_Spike_StickAround,
		Projectile_Spike_StickAround_Aoe,
		ProjectileEnd,
		#endregion

		#region Summons
		#endregion
	}

	public enum GameState
	{
		Waiting,
		Started,
		Ended,
	}

	[Flags]
	public enum CCFlags
	{
		None = 0,
		Knockback = 1,
		Stun = 2,
	}

	public enum TeamType
	{
		None,
		Red,
		Blue,
	}
}


