namespace Server.Game
{
	public readonly struct PlayerInput
	{
		public readonly long ClientTargetTick { get; init; }
		public readonly long ReceivedTick { get; init; }
		public readonly float LookDirX { get; init; }
		public readonly float LookDirY { get; init; }
		public readonly float MoveDirX { get; init; }
		public readonly float MoveDirY { get; init; }
		public readonly ushort ButtonPressed { get; init; }
		public static void Combine(in PlayerInput left, in PlayerInput right, out PlayerInput res)
		{
			res = new PlayerInput
			{
				LookDirX = (left.LookDirX + right.LookDirX) / 2,
				LookDirY = (left.LookDirY + right.LookDirY) / 2,
				MoveDirX = (left.MoveDirX + right.MoveDirX) / 2,
				MoveDirY = (left.MoveDirY + right.MoveDirY) / 2,
				ButtonPressed = (ushort)(left.ButtonPressed | right.ButtonPressed),
			};
		}
	}
}
