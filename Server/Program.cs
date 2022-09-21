using Server.DB;
using Server.Log;
using ServerCore;
using ServerCore.Managers;
using System;
using System.Threading;
using static Server.Utils.Enums;
using static ServerCore.Utils.Tools;


namespace Server
{
	public class Program
	{
		public static Action Update { get; set; }
		const long WaitTick = 250;
		static void Main(string[] args)
		{
			#region Init
			LogMgr.Init();
			GameDBContext.Init(false);
			GameMgr.Init();
			MapMgr.Init();
			JobMgr.Init();
			#endregion



			Listener listener = new(socket => SessionMgr.GenerateSession<ClientSession>(socket));
			var endPoint = GetNewEndPoint(7777);
			listener.StartListen(endPoint);
			LogMgr.Log($"Listening to {endPoint}", TraceSourceType.Network, TraceSourceType.Console);
			long nowTick = default;
			while (true)
			{
				if (WaitTick < Environment.TickCount64 - nowTick)
				{
					Update.Invoke();
					nowTick = Environment.TickCount64;
				}

			}
		}
	}
}
