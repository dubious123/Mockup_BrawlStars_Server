namespace Server;

public class JobMgr
{
	private static JobMgr _instance = new();
	private ConcurrentDictionary<string, JobQueue> _jobDict = new();

	private JobMgr() { }

	public static void Init()
	{
		CreatejobQueue(Define.PacketParserQueueName, 1, true, 1);
		CreatejobQueue(Define.PacketHandlerQueueName, 1, true, 1);
		CreatejobQueue(Define.PacketSendQueueName, 1, true, 1);
		CreatejobQueue(Define.PacketGameQueueName, 1, true, 1);
	}

	public static JobQueue CreatejobQueue(string name, int waitTick, bool startNow, int threadNum)
	{
		JobQueue queue = new(name, threadNum, waitTick);
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
