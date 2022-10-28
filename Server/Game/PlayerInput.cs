namespace Server.Game
{
	public readonly record struct PlayerInput
	{
		public readonly long ClientTargetTick { get; init; }
		public readonly long ReceivedTick { get; init; }
		public readonly sfloat LookDirX { get; init; }
		public readonly sfloat LookDirY { get; init; }
		public readonly sfloat MoveDirX { get; init; }
		public readonly sfloat MoveDirY { get; init; }
		public readonly ushort ButtonPressed { get; init; }
		public static void Combine(in PlayerInput left, in PlayerInput right, out PlayerInput res)
		{
			res = new PlayerInput
			{
				LookDirX = (left.LookDirX + right.LookDirX) / sfloat.Two,
				LookDirY = (left.LookDirY + right.LookDirY) / sfloat.Two,
				MoveDirX = (left.MoveDirX + right.MoveDirX) / sfloat.Two,
				MoveDirY = (left.MoveDirY + right.MoveDirY) / sfloat.Two,
				ButtonPressed = (ushort)(left.ButtonPressed | right.ButtonPressed),
			};
		}
	}
}
