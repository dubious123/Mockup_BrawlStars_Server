namespace Server.Utils
{
	public class CoroutineHelper
	{
		private List<IEnumerator<int>> _coroutines = new();
		private Dictionary<int, int> _pauseDict = new();
		private Dictionary<int, int> _activeDict = new();
		private Stack<int> _holes = new();
		private Stack<int> _tempHoles = new();
		private int _currentIndex;

		public void Update()
		{
			if (_activeDict.Count == 0) return;
			foreach (var key in _activeDict.Keys)
			{
				_currentIndex = key;
				var value = _activeDict[key];
				if (value > 0)
				{
					_activeDict[key] = value - 1;
					continue;
				}

				var coroutine = _coroutines[key];
				if (coroutine.MoveNext())
				{
					_activeDict[key] = coroutine.Current;
					continue;
				}

				_coroutines[key] = null;
				_tempHoles.Push(key);
			}

			if (_tempHoles.Count == 0) return;
			while (_tempHoles.TryPop(out var index))
			{
				_activeDict.Remove(index);
				_holes.Push(index);
			}
		}

		public int RunCoroutine(IEnumerator<int> coroutine)
		{
			if (_holes.TryPeek(out int index))
			{
				_coroutines[index] = coroutine;
				_activeDict.Add(index, 0);
				return index;
			}
			index = _coroutines.Count;
			_coroutines.Add(coroutine);
			_activeDict.Add(index, 0);
			return index;
		}

		public int RunDelayed(float delay, IEnumerator<int> coroutine)
		{
			return RunCoroutine(BuildDelayedCoroutine(delay, coroutine));
		}

		public int WaitForFrames(int delay, IEnumerator<int> coroutine)
		{
			return RunCoroutine(BuildWaitFrameCoroutine(delay, coroutine));
		}

		public int WaitUntilDone(IEnumerator<int> others)
		{
			RunCoroutine(AppendAction(others, () => _activeDict[_currentIndex] = 0));
			return int.MaxValue;
		}

		public bool Pause(int id)
		{
			if (_pauseDict.ContainsKey(id) || (_activeDict.ContainsKey(id) == false)) return false;
			_activeDict.Remove(id, out var value);
			_pauseDict.Add(id, value);
			return true;
		}

		public void PauseAll()
		{
			_activeDict.ToList().ForEach(x => _pauseDict.Add(x.Key, x.Value));
			_activeDict.Clear();
		}

		public bool Resume(int id)
		{
			if (_pauseDict.Remove(id, out var value) == false) return false;
			_activeDict.Add(id, value);
			return true;
		}

		public bool Kill(int id)
		{
			bool res = true;
			res &= _pauseDict.Remove(id);
			res &= _activeDict.Remove(id);
			res &= _coroutines.Count > id;
			if (res) _coroutines[id] = null;
			return res;
		}

		public void KillAll()
		{
			_pauseDict.Clear();
			_activeDict.Clear();
			_coroutines.Clear();
		}

		private static IEnumerator<int> BuildDelayedCoroutine(float delay, IEnumerator<int> enumerator)
		{
			float now = 0f;
			while (now < delay)
			{
				now += Timing.DeltaTime;
				yield return 0;
			}

			while (enumerator.MoveNext())
			{
				yield return enumerator.Current;
			}
		}

		private static IEnumerator<int> BuildWaitFrameCoroutine(int waitFrame, IEnumerator<int> enumerator)
		{
			yield return waitFrame;
			while (enumerator.MoveNext())
			{
				yield return enumerator.Current;
			}
		}

		private static IEnumerator<int> AppendAction(IEnumerator<int> enumerator, Action action)
		{
			while (enumerator.MoveNext())
			{
				yield return enumerator.Current;
			}

			action.Invoke();
		}
	}
}
