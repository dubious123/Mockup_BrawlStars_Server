using System.Net.Security;

namespace Server
{
	public class Program
	{
		private const long WaitTick = 166667;

		public static ConcurrentAction Update { get; set; } = new();

		private static void Main(string[] args)
		{
			#region Init
			LogMgr.Init();
			GameDBContext.Init(false);
			GameMgr.Init();
			MapMgr.Init();
			JobMgr.Init();
			Timing.Init();
			#endregion
			Listener listener = new(socket => SessionMgr.GenerateSession<ClientSession>(socket));
			var endPoint = GetNewEndPoint(7777);
			listener.StartListen(endPoint);
			LogMgr.Log($"Listening to {endPoint}", TraceSourceType.Network, TraceSourceType.Console);
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
}
