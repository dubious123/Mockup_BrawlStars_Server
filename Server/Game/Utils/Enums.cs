using System;

public static partial class Enums
{
	public enum NetObjectType
	{
		None,

		#region Character
		Character_Shelly,
		#endregion

		#region Env
		Env_Wall,
		Env_Tree,
		#endregion

		#region Projectile
		Projectile_Shelly_Buckshot,
		Projectile_Shelly_SuperShell,
		#endregion

		#region Summons
		#endregion
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


