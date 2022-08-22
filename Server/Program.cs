using ServerCore;
using ServerCore.Managers;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using static ServerCore.Utils.Tools;

namespace Server
{
	class Program
	{
		static void Main(string[] args)
		{
			Listener listener = new Listener(socket => SessionMgr.GenerateSession<ClientSession>(socket));
			listener.StartListen(GetNewEndPoint(7777));
			Console.WriteLine("Listening...");
			Thread.Sleep(-1);

		}
	}
}
