using Server.DB;
using Server.Log;
using ServerCore;
using ServerCore.Managers;
using System;
using System.Threading;
using static Server.Utils.Enums;
using static ServerCore.Utils.Tools;
using System.Diagnostics;
using Server.Utils;

namespace Server
{
	public class Program
	{
		public static ConcurrentAction Update { get; set; } = new();
		const long WaitTick = 166667;
		static void Main(string[] args)
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
			//frame start 
			//last = now;
			//
			//base =  environment.tickCount;
			//frame end
			//new frame start
			//delta time = base - environment.tickCount;
			Stopwatch sw = new Stopwatch();
			while (true)
			{
				delta = DateTime.Now.Ticks - nowTick;

				if (WaitTick <= delta)
				{
					//LogMgr.Log($"{delta}", TraceSourceType.Network);
					nowTick = DateTime.Now.Ticks;
					Timing.OnNewFrameStart(nowTick);
					Update.Invoke();
					//sw.Stop();
					//LogMgr.Log($"time : {sw.ElapsedMilliseconds}, tick : {sw.ElapsedTicks}", TraceSourceType.Network);
					//sw.Restart();
				}

			}
		}
	}
}
