using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualBasic;

namespace Server.Game
{
	public class NTiming : INetUpdatable
	{
		private DetDictionary<int, NetCoroutine> _coDict = new();
		private int index = 0;

		public void RunCoroutine(IEnumerator<int> coroutine)
		{
			_coDict.Add(index, new NetCoroutine(index, coroutine));
			++index;
		}

		public void CallDelayed(int delay, Action action)
		{
			RunCoroutine(BaseEnumerator(delay, action));
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
				if (_current == 0)
				{
					var res = _coroutine.MoveNext();
					_current = _coroutine.Current;
					return res;
				}
				else
				{
					--_current;
					return true;
				}
			}
		}

		public IEnumerator<int> BaseEnumerator(int delay, Action action = null)
		{
			yield return delay;
			action?.Invoke();
			yield break;
		}
	}
}
