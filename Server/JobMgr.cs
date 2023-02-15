namespace Server;

public class JobMgr
{
	private static JobMgr _instance = new();
	private ConcurrentDictionary<string, JobQueue> _jobDict = new();

	private JobMgr() { }

	public static void Init()
	{
		CreatejobQueue(Define.PacketSendQueueName, true, double.Epsilon);
	}

	public static JobQueue CreatejobQueue(string name, bool startNow, double interval)
	{
		JobQueue queue = new(name, interval);
		if (_instance._jobDict.TryGetValue(name, out _)) throw new Exception();
		_instance._jobDict[name] = queue;
		if (startNow)
			queue.Start();
		return queue;
	}

	public static void Push(string name, Action action)
	{
		if (_instance._jobDict.ContainsKey(name) == false) throw new Exception();
		_instance._jobDict[name].Push(action);
	}

	public static JobQueue GetQueue(string name)
	{
		return _instance._jobDict[name];
	}
}
