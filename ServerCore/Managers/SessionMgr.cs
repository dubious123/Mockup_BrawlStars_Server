using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;

namespace ServerCore.Managers
{
	public class SessionMgr
	{
		static int _sessionCount = 0;
		static readonly ConcurrentDictionary<int, Session> _sessionDict = new();
		public static int GetSessionId()
		{
			return Interlocked.Increment(ref _sessionCount);
		}
		public static T GenerateSession<T>(Socket socket) where T : Session, new()
		{
			T session = new();
			var id = GetSessionId();
			if (_sessionDict.TryAdd(id, session) == false)
				throw new Exception();
			session.Init(id, socket);
			return session;
		}
		//public void Flush_Send()
		//{
		//	foreach (var session in _sessionDict.Values)
		//	{
		//		if (session.SendRegistered)
		//			session.Send();
		//	}
		//	JobMgr.Inst.Push("Send", Flush_Send);
		//}
		public static Session Remove(int id)
		{
			_sessionDict.TryRemove(id, out Session session);
			return session;
		}
		public static Session Find(int id)
		{
			_sessionDict.TryGetValue(id, out Session session);
			return session;
		}
		public static void Close(int id)
		{
			Remove(id)?.Close();
		}
		public static void CloseAll()
		{
			foreach (var session in _sessionDict.Values)
			{
				session.Close();
				Remove(session.Id);
			}
		}
	}
}
