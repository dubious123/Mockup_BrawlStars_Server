namespace Server.Utils
{
	public class ConcurrentAction
	{
		ConcurrentDictionary<int, Action> _actionDict = new();

		public void Invoke()
		{
			if (_actionDict.IsEmpty) return;
			foreach (var action in _actionDict.Values)
			{
				action.Invoke();
			}
			//var enumerator = _actionDict.Values.GetEnumerator();
			//while (enumerator.MoveNext())
			//{
			//	enumerator.Current.Invoke();
			//}
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
		void AddAction(Action action)
		{
			_actionDict.TryAdd(action.GetHashCode(), action);
		}
		void DeleteAction(Action action)
		{
			_actionDict.TryRemove(action.GetHashCode(), out _);
		}

	}
}
