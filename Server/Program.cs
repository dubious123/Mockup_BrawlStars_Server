using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using static ServerCore.Utils.Tools;

namespace Server
{
	class Program
	{
		static void Main(string[] args)
		{
			Listener listener = new Listener();
			listener.StartListen(GetNewEndPoint(7777));
			//_listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
			Console.WriteLine("Listening...");


		}
	}
}
