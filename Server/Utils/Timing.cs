namespace Server.Utils;


public class Timing
{
	private static Timing _instnace = new();

	private Timing() { }

	public static sfloat DeltaTime { get; private set; }

	public static ulong Fps { get; private set; }

	public static void Init()
	{
		Fps = 0;
		DeltaTime = sfloat.One / (sfloat)60f;
	}

	public static void OnNewFrameStart(long deltaTime)
	{
		// DeltaTime = deltaTime;
		Fps++;
	}
}
