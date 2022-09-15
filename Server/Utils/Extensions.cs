using Server.Game;
using Server.Log;
using System.Diagnostics;

namespace Server.Utils
{
	public static class Extensions
	{
		public static TraceSource AddConsoleListener(this TraceSource ts, TraceOptions options)
		{
			return LogMgr.AddConsoleListener(ts, options);
		}
		public static TraceSource AddTextWriterListener(this TraceSource ts, string dirPath, string fileName, TraceOptions options)
		{
			return LogMgr.AddTextWriterListener(ts, dirPath, fileName, options);
		}
		public static TraceSource AddListener(this TraceSource ts, TraceListener listener)
		{

			return LogMgr.AddListenter(ts, listener);
		}
	}
}
