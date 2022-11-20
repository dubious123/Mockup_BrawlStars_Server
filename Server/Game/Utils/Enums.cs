public static partial class Enums
{
	public enum NetObjectId
	{
		Wall,
		Player_Dog,
	}

	public enum NetObjectTag
	{
		Character = 0,
		Wall = 1,
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


