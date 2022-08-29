﻿using System;
using System.Threading;
using System.Net.Sockets;
using static ServerCore.Utils.Tools;
using ServerCore;
using ServerCore.Managers;
using System.IO;

namespace TestClient
{
	class Program
	{
		static void Main(string[] args)
		{
			Connector connector = new Connector(socket => SessionMgr.GenerateSession<ServerSession>(socket));
			connector.StartConnect(GetNewEndPoint(7777));
			Console.WriteLine("Connecting...");


			Thread.Sleep(-1);
		}
	}
}
