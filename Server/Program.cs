using Server.DB;
using ServerCore;
using ServerCore.Managers;
using ServerCore.Packets;
using System;
using System.Threading;
using static ServerCore.Utils.Tools;

namespace Server
{
	class Program
	{
		static void Main(string[] args)
		{
			GameDBContext.Init(false);
			Listener listener = new Listener(socket => SessionMgr.GenerateSession<ClientSession>(socket));
			var endPoint = GetNewEndPoint(7777);
			listener.StartListen(endPoint);
			Console.WriteLine($"Listening to {endPoint}");
			Thread.Sleep(-1);
		}
	}
}
