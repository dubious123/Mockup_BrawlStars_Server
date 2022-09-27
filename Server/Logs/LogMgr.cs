using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Concurrent;
using Server.Utils;
using static Server.Utils.Enums;

namespace Server.Log
{
	public class LogMgr
	{
		static LogMgr _instance = new();
		static string _dirPath;
		TraceSource[] _tsArr;
		DateTime _dt;
		LogMgr()
		{
			_dt = DateTime.Now;
			_dirPath = Directory.GetCurrentDirectory() + "/../../../Logs";
			Directory.CreateDirectory(_dirPath);
			_tsArr = new TraceSource[9];
			_tsArr[0] = BuildNewTraceSource(Define.Ts_Packet)
					.AddTextWriterListener(string.Empty, "PacketLogs.txt", TraceOptions.DateTime);
			_tsArr[1] = BuildNewTraceSource(Define.Ts_Network)
					.AddTextWriterListener(string.Empty, "NetworkLogs.txt", TraceOptions.DateTime);
			_tsArr[2] = BuildNewTraceSource(Define.Ts_Session)
					.AddTextWriterListener(string.Empty, "SessionLogs.txt", TraceOptions.DateTime);
			_tsArr[3] = BuildNewTraceSource(Define.Ts_Handler)
					.AddTextWriterListener(string.Empty, "PacketHandlerLogs.txt", TraceOptions.DateTime);
			_tsArr[4] = BuildNewTraceSource(Define.Ts_Console)
					.AddConsoleListener(TraceOptions.DateTime);
			_tsArr[5] = BuildNewTraceSource(Define.Ts_Error)
					.AddTextWriterListener(string.Empty, "Errors.txt", TraceOptions.DateTime | TraceOptions.Callstack)
					.AddConsoleListener(TraceOptions.DateTime | TraceOptions.Callstack);
			var listener = _tsArr[0].Listeners[0];
			_tsArr[6] = BuildNewTraceSource(Define.Ts_PacketSend)
					.AddTextWriterListener(string.Empty, "SendLog.txt")
					.AddListener(listener);
			_tsArr[7] = BuildNewTraceSource(Define.Ts_PacketRecv)
					.AddTextWriterListener(string.Empty, "RecvLog.txt")
					.AddListener(listener);
			_tsArr[(int)TraceSourceType.Debug] = BuildNewTraceSource(Define.Ts_Debug)
					.AddTextWriterListener(string.Empty, "Debug.txt");


			Trace.AutoFlush = true;
			//AppDomain.CurrentDomain.ProcessExit += new EventHandler((obj, e) =>
			//{
			//	Log("hi", TraceEventType.Information, TraceSourceType.Network);
			//	foreach (var ts in _tsArr)
			//	{
			//		ts.Flush();
			//	}
			//});


		}
		~LogMgr()
		{
			foreach (var ts in _tsArr)
			{
				ts.Flush();
			}
		}
		public static void Init() { }

		public static void Log(string message, TraceEventType eventType, params TraceSourceType[] sourceTypes)
		{
			foreach (var sourceType in sourceTypes)
			{
				_instance._tsArr[(int)sourceType].TraceEvent(eventType, default, message);
			}
			if (eventType == TraceEventType.Error && sourceTypes.Contains(TraceSourceType.Error) == false)
			{
				_instance._tsArr[4].TraceEvent(eventType, default, message);
			}
		}
		public static void Log(string message, params TraceSourceType[] sourceTypes)
		{
			Log($"\n[Date = {DateTime.Now}.{DateTime.Now.Millisecond.ToString("000")}]\n{message}", TraceEventType.Information, sourceTypes);
		}

		static TraceSource BuildNewTraceSource(string name)
		{
			TraceSource ts = new(name);
			ts.Listeners.Clear();
			ts.Switch = new SourceSwitch(name + "Switch", "Information");
			return ts;
		}
		public static TraceSource AddConsoleListener(TraceSource ts, TraceOptions options)
		{
			ConsoleTraceListener listener = new();
			listener.Name = ts.Name;
			listener.TraceOutputOptions = options;
			ts.Listeners.Add(listener);
			return ts;
		}
		public static TraceSource AddTextWriterListener(TraceSource ts, string dirPath, string fileName, TraceOptions options = TraceOptions.None)
		{
			var path = Path.Combine(_dirPath, dirPath);
			Directory.CreateDirectory(path);
			FileStream stream = new(Path.Combine(path, fileName), FileMode.Create, FileAccess.Write);
			TextWriterTraceListener listener = new(stream);
			listener.Name = ts.Name;
			listener.TraceOutputOptions = options;
			ts.Listeners.Add(listener);
			return ts;
		}
		public static TraceSource AddListenter(TraceSource ts, TraceListener listener)
		{
			ts.Listeners.Add(listener);
			return ts;
		}
	}
}
