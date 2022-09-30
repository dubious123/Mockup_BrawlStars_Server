using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Utils
{
	public class CoroutineHelper
	{
		private List<IEnumerator<float>> _coroutines = new();
		private HashSet<int> _pausedSet = new();
		private HashSet<int> _activeSet = new();
		private Stack<int> _holes = new();
		private Stack<int> _tempHoles = new();
		public void Update()
		{
			if (_activeSet.Count == 0) return;
			foreach (var index in _activeSet)
			{
				var current = _coroutines[index];
				if (current.MoveNext()) continue;
				_coroutines[index] = null;
				_tempHoles.Push(index);
			}
			if (_tempHoles.Count == 0) return;
			while (_tempHoles.TryPop(out var index))
			{
				_activeSet.Remove(index);
				_holes.Push(index);
			}
		}
		public int RunCoroutine(IEnumerator<float> coroutine)
		{
			if (_holes.TryPeek(out int index))
			{
				_coroutines[index] = coroutine;
				_activeSet.Add(index);
				return index;
			}
			index = _coroutines.Count;
			_coroutines.Add(coroutine);
			_activeSet.Add(index);
			return index;
		}
		public int RunDelayed(float delay, IEnumerator<float> coroutine)
		{
			return RunCoroutine(BuildDelayedCoroutine(delay, coroutine));
		}
		public bool Pause(int id)
		{
			if (_pausedSet.Contains(id) || (_activeSet.Contains(id) == false)) return false;
			_pausedSet.Add(id);
			_activeSet.Remove(id);
			return true;
		}
		public void PauseAll()
		{
			_pausedSet.UnionWith(_activeSet);
			_activeSet.Clear();
		}
		public bool Resume(int id)
		{
			if (_pausedSet.Remove(id) == false) return false;
			_activeSet.Add(id);
			return true;
		}
		public bool Kill(int id)
		{
			bool res = false;
			res |= _pausedSet.Remove(id);
			res |= _activeSet.Remove(id);
			res |= _coroutines.Count > id;
			if (res) _coroutines[id] = null;
			return res;
		}
		public void KillAll()
		{
			_pausedSet.Clear();
			_activeSet.Clear();
			_coroutines.Clear();
		}
		static IEnumerator<float> BuildDelayedCoroutine(float delay, IEnumerator<float> enumerator)
		{
			float now = 0f;
			while (now < delay)
			{
				now += Timing.DeltaTime;
				yield return 0f;
			}
			while (enumerator.MoveNext())
			{
				yield return 0f;
			}
		}
	}


}
