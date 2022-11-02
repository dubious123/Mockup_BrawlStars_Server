public readonly struct InputData
{
	public readonly sVector3 MoveInput { get; init; }
	public readonly sVector3 LookInput { get; init; }
	public readonly ushort ButtonInput { get; init; }

	public static void Combine(in InputData left, in InputData right, out InputData res)
	{
		res = new InputData
		{
			MoveInput = (left.MoveInput + right.MoveInput) * (sfloat)0.5f,
			LookInput = (left.LookInput + right.LookInput) * (sfloat)0.5f,
			ButtonInput = (ushort)(left.ButtonInput | right.ButtonInput),
		};
	}
}
