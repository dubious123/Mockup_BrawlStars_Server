namespace Server.Log
{
	public class LogMgr
	{
		private static LogMgr _instance = new();
		private static string _dirPath;
		private TraceSource[] _tsArr;
		private DateTime _dt;

		private LogMgr()
		{
			_dt = DateTime.Now;
			_dirPath = Directory.GetCurrentDirectory() + "/../../../Logs";
			Directory.CreateDirectory(_dirPath);
			_tsArr = new TraceSource[10];
			_tsArr[0] = BuildNewTraceSource(Define.TsPacket)
					.AddTextWriterListener(string.Empty, "PacketLogs.txt", TraceOptions.DateTime);
			_tsArr[1] = BuildNewTraceSource(Define.TsNetwork)
					.AddTextWriterListener(string.Empty, "NetworkLogs.txt", TraceOptions.DateTime);
			_tsArr[2] = BuildNewTraceSource(Define.TsSession)
					.AddTextWriterListener(string.Empty, "SessionLogs.txt", TraceOptions.DateTime);
			_tsArr[3] = BuildNewTraceSource(Define.TsHandler)
					.AddTextWriterListener(string.Empty, "PacketHandlerLogs.txt", TraceOptions.DateTime);
			_tsArr[4] = BuildNewTraceSource(Define.TsConsole)
					.AddConsoleListener(TraceOptions.DateTime);
			_tsArr[5] = BuildNewTraceSource(Define.TsError)
					.AddTextWriterListener(string.Empty, "Errors.txt", TraceOptions.DateTime | TraceOptions.Callstack)
					.AddConsoleListener(TraceOptions.DateTime | TraceOptions.Callstack);
			var listener = _tsArr[0].Listeners[0];
			_tsArr[6] = BuildNewTraceSource(Define.TsPacketSend)
					.AddTextWriterListener(string.Empty, "SendLog.txt")
					.AddListener(listener);
			_tsArr[7] = BuildNewTraceSource(Define.TsPacketRecv)
					.AddTextWriterListener(string.Empty, "RecvLog.txt")
					.AddListener(listener);
			_tsArr[(int)TraceSourceType.Debug] = BuildNewTraceSource(Define.TsDebug)
					.AddTextWriterListener(string.Empty, "Debug.txt");
			_tsArr[(int)TraceSourceType.Game] = BuildNewTraceSource(Define.TsGame)
					.AddTextWriterListener(string.Empty, "Game.txt");
			Trace.AutoFlush = true;
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

		public static void LogInfo(string message, int id, params TraceSourceType[] sourceTypes)
		{
			foreach (var sourceType in sourceTypes)
			{
				_instance._tsArr[(int)sourceType].TraceEvent(TraceEventType.Information, id, $"\n[Date = {DateTime.Now}.{DateTime.Now.Millisecond.ToString("000")}]\n{message}");
			}
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

		private static TraceSource BuildNewTraceSource(string name)
		{
			TraceSource ts = new(name);
			ts.Listeners.Clear();
			ts.Switch = new SourceSwitch(name + "Switch", "Information");
			return ts;
		}
	}
}
