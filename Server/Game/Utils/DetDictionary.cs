using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
	public class DetDictionary<TKey, TValue> : IEnumerable<TValue>
	{
		private readonly Dictionary<TKey, TValue> _dictionary = new();
		private readonly LinkedList<TValue> _list = new();

		public ICollection<TKey> Keys => _dictionary.Keys;
		public ICollection<TValue> Values => _dictionary.Values;
		public TValue this[TKey key] { get => _dictionary[key]; set => _dictionary[key] = value; }
		public int Count => _dictionary.Count;

		public bool TryAdd(TKey key, TValue value)
		{
			var res = _dictionary.TryAdd(key, value);
			if (res)
			{
				_list.AddLast(value);
			}

			return res;
		}

		public void Add(TKey key, TValue value)
		{
			_dictionary.Add(key, value);
			_list.AddLast(value);
		}

		public bool ContainsKey(TKey key)
			=> _dictionary.ContainsKey(key);

		public bool Remove(TKey key)
			=> _dictionary.Remove(key);

		public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
			=> _dictionary.TryGetValue(key, out value);

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			_dictionary.Add(item.Key, item.Value);
			_list.AddLast(item.Value);
		}

		public void Clear()
		{
			_dictionary.Clear();
			_list.Clear();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
			=> _dictionary.Contains(item);

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			_list.Remove(item.Value);
			return _dictionary.Remove(item.Key);
		}

		IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
			=> _list.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> _list.GetEnumerator();
	}
}
