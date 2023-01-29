using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Linq;

using ICSharpCode.Decompiler.Util;

namespace Server;

public class GameLoopQueue
{
	public bool IsFull => _isFull;
	private System.Timers.Timer _timer;
	private ConcurrentQueue<GameRoom> _roomQueue = new();
	private bool _isFull = false;
	public GameLoopQueue()
	{
		_timer = new(double.Epsilon);
		_timer.Elapsed += Loop;
		_timer.AutoReset = false;
	}

	public void Push(GameRoom room)
	{
		_roomQueue.Enqueue(room);
		if (_roomQueue.IsEmpty is false)
		{
			_timer.Start();
		}
	}

	private void Loop(object sender, ElapsedEventArgs e)
	{
		for (int i = _roomQueue.Count; i > 0; --i)
		{
			if (_roomQueue.TryDequeue(out var room))
			{
				room.Update();
				if (room.State != GameState.Ended)
				{
					_roomQueue.Enqueue(room);
				}
			}
		}

		if ((e.SignalTime - DateTime.Now).TotalMilliseconds > 100)
		{
			Loggers.Debug.Information("GameLoop full");
			_isFull = true;
		}

		_timer.Start();
	}
}
