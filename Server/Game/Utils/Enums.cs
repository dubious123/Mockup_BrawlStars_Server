public static partial class Enums
{
	public enum NetObjectType
	{
		None,

		#region Env
		Env_Wall,
		Env_Tree,
		#endregion

		#region Character
		Character_Shelly,
		Character_Spike,
		#endregion

		#region Projectile
		Projectile_Shelly_Buckshot,
		Projectile_Shelly_SuperShell,
		Projectile_Spike_NeedleGranade,
		Projectile_Spike_NeedleGranade_Needle,
		Projectile_Spike_StickAround,
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


