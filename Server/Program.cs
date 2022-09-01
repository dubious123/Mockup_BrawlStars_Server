using ServerCore;
using ServerCore.Managers;
using ServerCore.Packets;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using static ServerCore.Utils.Tools;

namespace Server
{
	class Program
	{
		static void Main(string[] args)
		{
			Listener listener = new Listener(socket => SessionMgr.GenerateSession<ClientSession>(socket));
			var endPoint = GetNewEndPoint(7777);
			listener.StartListen(endPoint);
			Console.WriteLine($"Listening to {endPoint}");
			Thread.Sleep(-1);
		}
	}
}
