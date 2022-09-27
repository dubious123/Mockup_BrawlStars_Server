using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Server
{
	public class JobQueue
	{
		readonly string _name;
		readonly int _waitTick;

		bool _isJobQueueRunning;
		Thread[] _threads;
		ConcurrentQueue<Action> _jobQueue = new();
		public JobQueue(string name, int threadNum, int waitTick)
		{
			_name = name;
			_waitTick = waitTick;
			_threads = new Thread[threadNum];
			for (int i = 0; i < threadNum; i++)
			{
				_threads[i] = new(Loop);
				_threads[i].Name = $"{_name}[{i}]";
			}
		}
		public void Start()
		{
			foreach (var t in _threads)
			{
				_isJobQueueRunning = true;
				t.Start();
			}
		}
		public void Push(Action action)
		{
			_jobQueue.Enqueue(action);
		}
		public void Stop()
		{
			_isJobQueueRunning = false;
		}
		public void Resume()
		{
			_isJobQueueRunning = true;
			foreach (var t in _threads)
			{
				t.Interrupt();
			}
		}
		void Loop()
		{
		Loop:
			long nowTick = 0;
			try
			{

				while (_isJobQueueRunning)
				{
					if (_waitTick < DateTime.UtcNow.Ticks - nowTick)
					{
						for (int j = 0; j < _jobQueue.Count; j++)
						{
							if (_jobQueue.TryDequeue(out var action))
								action.Invoke();
						}
						nowTick = DateTime.UtcNow.Ticks;
					}
				}
				Thread.Sleep(Timeout.Infinite);
			}
			catch (ThreadInterruptedException)
			{
				goto Loop;
			}
		}
	}
}
