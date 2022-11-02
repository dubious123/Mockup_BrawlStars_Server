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
}