namespace Server.Utils
{
	public sealed class JEvent
	{
		private Dictionary<string, Action> _actionDict = new();
		private Action _defaultAction;

		public void AddListener(Action callback)
		{
			_defaultAction += callback;
		}

		public void AddListener(string tag, Action callback)
		{
			if (_actionDict.TryGetValue(tag, out var action) == false)
			{
				_actionDict.Add(tag, callback);
				return;
			}

			action += callback;
		}

		public Action RemoveListener(string tag)
		{
			_actionDict.Remove(tag, out var action);
			return action;
		}

		public void Clear()
		{
			_defaultAction = null;
			_actionDict.Clear();
		}

		public void Invoke()
		{
			_defaultAction?.Invoke();
			foreach (var action in _actionDict.Values)
			{
				action.Invoke();
			}
		}
	}
}
