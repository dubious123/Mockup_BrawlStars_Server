namespace Server.Utils
{
	public class ConcurrentAction
	{
		private ConcurrentDictionary<int, Action> _actionDict = new();

		public void Invoke()
		{
			if (_actionDict.IsEmpty) return;
			foreach (var action in _actionDict.Values)
			{
				action.Invoke();
			}
		}

		private void AddAction(Action action)
		{
			_actionDict.TryAdd(action.GetHashCode(), action);
		}

		private void DeleteAction(Action action)
		{
			_actionDict.TryRemove(action.GetHashCode(), out _);
		}

		public static ConcurrentAction operator +(ConcurrentAction cAction, Action action)
		{
			cAction.AddAction(action);
			return cAction;
		}

		public static ConcurrentAction operator -(ConcurrentAction cAction, Action action)
		{
			cAction.DeleteAction(action);
			return cAction;
		}
	}
}
