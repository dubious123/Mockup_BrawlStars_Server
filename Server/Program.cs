using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Sinks.File;
using Serilog.Sinks.SystemConsole.Themes;

using Server.Logs;

namespace Server;

public class Program
{
	private const long WaitTick = 166667;

	public static ConcurrentAction Update { get; set; } = new();

	private static void Main(string[] args)
	{
		#region Init
		Loggers.Init();
		GameDBContext.Init(false);
		GameMgr.Init();
		DataMgr.Init();
		JobMgr.Init();
		Timing.Init();
		#endregion
		Listener listener = new(socket => SessionMgr.GenerateSession<ClientSession>(socket));
		var endPoint = GetNewEndPoint(7777);
		listener.StartListen(endPoint);
		Loggers.Console.Information("Listening to {0}", endPoint);
		long nowTick = default;
		long delta = default;
		while (true)
		{
			delta = DateTime.UtcNow.Ticks - nowTick;
			if (delta >= WaitTick)
			{
				nowTick = DateTime.UtcNow.Ticks;
				Update.Invoke();
				Timing.OnNewFrameStart(nowTick);
			}
		}
	}
}
