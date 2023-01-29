using System.Net;

namespace Server;

public class Program
{
	private const long WaitTick = 166667;

	public static ConcurrentAction Update { get; set; } = new();

	private static void Main(string[] args)
	{
		#region Init
		Config.Init();
		Loggers.Init();
		GameDBContext.Init(false);
		GameMgr.Init();
		DataMgr.Init();
		JobMgr.Init();
		SessionMgr.Init();
		#endregion
		Listener listener = new(socket => SessionMgr.GenerateSession<ClientSession>(socket));
		var endPoint = new IPEndPoint(Config.CONNECT_ADDRESS, 7777);
		listener.StartListen(endPoint);
		Loggers.Console.Information("Listening to {0}", endPoint);
		//var GameLoop = JobMgr.GetQueue(Define.GameQueueName);
		//long nowTick = default;
		//long delta = default;
		//while (true)
		//{
		//	delta = DateTime.UtcNow.Ticks - nowTick;
		//	if (delta >= WaitTick)
		//	{
		//		nowTick = DateTime.UtcNow.Ticks;
		//		Update.Invoke();
		//	}
		//}
		Console.ReadLine();
	}
}
