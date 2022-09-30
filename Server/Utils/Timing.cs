using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Timers;

namespace Server.Utils
{
	public class Timing
	{
		static Timing _instnace = new();
		Timing() { }
		public static float DeltaTime { get; private set; }
		public static ulong Fps { get; private set; }
		public static void Init()
		{
			Fps = 0;
			DeltaTime = 1 / 60f;
		}
		public static void OnNewFrameStart(long deltaTime)
		{
			//DeltaTime = deltaTime;
			Fps++;
		}
	}
}
