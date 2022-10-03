using Tools = Server.Game.Base.Utils.Tools;

namespace Server.Utils
{
	public static class Extensions
	{
		public static TraceSource AddConsoleListener(this TraceSource ts, TraceOptions options)
		{
			return LogMgr.AddConsoleListener(ts, options);
		}

		public static TraceSource AddTextWriterListener(this TraceSource ts, string dirPath, string fileName, TraceOptions options = TraceOptions.None)
		{
			return LogMgr.AddTextWriterListener(ts, dirPath, fileName, options);
		}

		public static TraceSource AddListener(this TraceSource ts, TraceListener listener)
		{
			return LogMgr.AddListenter(ts, listener);
		}

		public static Vector3 SmoothDamp(this ref Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
		{
			return Tools.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
		}

		public static Quaternion LookRotation(this ref Vector3 forward, Vector3 up)
		{
			return Tools.LookRotation(forward, up);
		}

		public static Quaternion RotateTowards(this ref Quaternion from, Quaternion to, float maxDegreesDelta)
		{
			return Tools.RotateTowards(from, to, maxDegreesDelta);
		}

		public static Vector3 Rotate(this in Quaternion rotation, in Vector3 origin)
		{
			return Tools.Rotate(rotation, origin);
		}
	}
}
