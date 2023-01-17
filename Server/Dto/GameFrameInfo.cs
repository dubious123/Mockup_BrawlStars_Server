using Server;

public class GameFrameInfo
{
	public GameFrameInfo(int playerCount)
	{
		Inputs = new InputData[playerCount];
	}

	public long StartTick { get; set; }
	public long TargetTick { get; set; }
	public InputData[] Inputs { get; private set; }

	public void Reset()
	{
		for (int i = 0; i < Inputs.Length; i++)
		{
			Inputs[i] = default;
		}
	}
}