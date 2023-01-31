namespace Server.Game
{
	public class NTiming : INetUpdatable
	{
		private DetDictionary<int, NetCoroutine> _coDict = new();
		private int index = 0;

		public int RunCoroutine(IEnumerator<int> coroutine)
		{
			_coDict.Add(index, new NetCoroutine(index, coroutine));
			return index++;
		}

		public int CallDelayed(int delay, Action action)
		{
			return RunCoroutine(BaseEnumerator(delay, action));
		}

		public void KillCoroutine(int index)
		{
			_coDict.Remove(index);
		}

		public void Update()
		{
			foreach (var co in _coDict)
			{
				if (co.MoveNext() is false)
				{
					_coDict.Remove(co.Index);
				}
			}
		}

		public void Reset()
		{
			_coDict.Clear();
			index = 0;
		}

		private class NetCoroutine
		{
			private IEnumerator<int> _coroutine;
			private int _current;
			private int _index;

			public int Index => _index;
			public int Current { set { _current = value; } }

			public NetCoroutine(int index, IEnumerator<int> coroutine)
			{
				_index = index;
				_coroutine = coroutine;
			}

			public bool MoveNext()
			{
				if (_current > 0)
				{
					--_current;
					return true;
				}
				else
				{
					var res = _coroutine.MoveNext();
					_current = _coroutine.Current;
					return res;
				}
			}
		}

		public IEnumerator<int> BaseEnumerator(int delay, Action action = null)
		{
			yield return --delay;
			action?.Invoke();
			yield break;
		}
	}
}
