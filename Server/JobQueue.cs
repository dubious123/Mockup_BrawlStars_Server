using System.Timers;

namespace Server;

public class JobQueue
{
	private readonly string _name;

	private ConcurrentQueue<Action> _jobQueue = new();
	private readonly System.Timers.Timer _timer;

	public JobQueue(string name, double interval)
	{
		_name = name;
		_timer = new System.Timers.Timer(interval);
		_timer.Elapsed += Loop;
	}

	public void Start()
	{
		_timer.Start();
	}

	public void Push(Action action)
	{
		_jobQueue.Enqueue(action);
	}

	public void Stop()
	{
		_timer.Stop();
	}

	private void Loop(Object source, ElapsedEventArgs e)
	{
		for (int j = 0; j < _jobQueue.Count; j++)
		{
			if (_jobQueue.TryDequeue(out var action))
				action.Invoke();
		}
	}
}
