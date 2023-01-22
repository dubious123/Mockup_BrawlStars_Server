using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;

namespace Server.Logs
{
	public static class Loggers
	{
		public static Logger Console;
		public static Logger Debug;
		public static Logger Game;
		public static Logger Send;
		public static Logger Recv;
		public static Logger Network;
		public static Logger Error;

		static Loggers()
		{
			var path = Directory.GetCurrentDirectory() + "/../../../Logs";
			DirectoryInfo di = new DirectoryInfo(path);
			if (di.Exists == false)
			{
				di.Create();
			}

			File.WriteAllText(path + "/Debug.txt", "");
			File.WriteAllText(path + "/Game.txt", "");
			File.WriteAllText(path + "/Send.txt", "");
			File.WriteAllText(path + "/Recv.txt", "");
			File.WriteAllText(path + "/Network.txt", "");
			File.WriteAllText(path + "/Error.txt", "");

			Console = new LoggerConfiguration().WriteTo.Console(theme: SystemConsoleTheme.Colored).CreateLogger();
			Debug = new LoggerConfiguration().WriteTo.File(path + "/Debug.txt").CreateLogger();
			Game = new LoggerConfiguration().WriteTo.File(path + "/Game.txt").CreateLogger();
			Send = new LoggerConfiguration().WriteTo.File(path + "/Send.txt").CreateLogger();
			Recv = new LoggerConfiguration().WriteTo.File(path + "/Recv.txt").CreateLogger();
			Network = new LoggerConfiguration().WriteTo.File(path + "/Network.txt").CreateLogger();
			Error = new LoggerConfiguration().WriteTo.File(path + "/Error.txt").CreateLogger();
		}

		public static void Init() { }
	}
}
