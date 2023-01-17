using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Game
{
	public class DetDictionary<TKey, TValue> : IEnumerable<TValue>
	{
		private struct Entry
		{
			public int Hash;    // Lower 31 bits of hash code, -1 if unused
			public int Next;        // Index of next entry, -1 if last
			public TKey Key;           // Key of entry
			public TValue Value;         // Value of entry
		}

		private int[] _buckets;
		private Entry[] _entries;
		private int _count;
		private int _holeIndex;
		private int _holeCount;

		public int Count => _count - _holeCount;

		public TValue this[TKey key]
		{
			get => _entries[FindEntry(key)].Value;
			set => Insert(key, value);
		}

		public DetDictionary() : this(7) { }

		public DetDictionary(int capacity)
		{
			Initialize(capacity);
		}

		public void Add(TKey key, TValue value)
		{
			Insert(key, value, true);
		}

		public bool TryAdd(TKey key, TValue value)
		{
			var hash = key.GetHashCode() & 0x7fff_ffff;
			var bucketIndex = hash % _buckets.Length;
			int targetEntryIndex;

			for (int entryIndex = _buckets[bucketIndex]; entryIndex >= 0; entryIndex = _entries[entryIndex].Next)
			{
				if (_entries[entryIndex].Hash == hash)
				{
					return false;
				}
			}

			if (_holeCount > 0)
			{
				targetEntryIndex = _holeIndex;
				_holeIndex = _entries[targetEntryIndex].Next;
				--_holeCount;
			}
			else
			{
				if (_count == _entries.Length)
				{
					Resize();
					bucketIndex = hash % _buckets.Length;
				}

				targetEntryIndex = _count++;
			}

			_entries[targetEntryIndex].Key = key;
			_entries[targetEntryIndex].Hash = hash;
			_entries[targetEntryIndex].Value = value;
			_entries[targetEntryIndex].Next = _buckets[bucketIndex];
			_buckets[bucketIndex] = targetEntryIndex;
			return true;
		}

		public void Clear()
		{
			if (Count == 0)
			{
				return;
			}

			for (int i = _buckets.Length - 1; i >= 0; --i)
			{
				_buckets[i] = -1;
			}

			Array.Clear(_entries, 0, _count);
			_holeIndex = -1;
			_count = 0;
			_holeCount = 0;
		}

		public bool ContainsKey(TKey key)
		{
			return FindEntry(key) >= 0;
		}

		public bool Remove(TKey key)
		{
			var hash = key.GetHashCode() & 0x7fff_ffff;
			var bucketIndex = hash % _buckets.Length;
			int last = -1;
			for (int i = _buckets[bucketIndex]; i >= 0; last = i, i = _entries[i].Next)
			{
				if (_entries[i].Hash != hash)
				{
					continue;
				}

				if (last < 0)
				{
					_buckets[bucketIndex] = _entries[i].Next;
				}
				else
				{
					_entries[last].Next = _entries[i].Next;
				}

				_entries[i].Key = default;
				_entries[i].Value = default;
				_entries[i].Hash = -1;
				_entries[i].Next = _holeIndex;
				_holeIndex = i;
				++_holeCount;
				return true;
			}

			return false;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			int i = FindEntry(key);
			if (i >= 0)
			{
				value = _entries[i].Value;
				return true;
			}

			value = default;
			return false;
		}

		private void Initialize(int capacity)
		{
			var size = PrimeHelper.GetPrime(capacity);
			_buckets = new int[size];
			for (int i = 0; i < size; i++)
			{
				_buckets[i] = -1;
			}

			_entries = new Entry[size];
			_holeIndex = -1;
		}

		private void Insert(TKey key, TValue value, bool add = false)
		{
			var hash = key.GetHashCode() & 0x7fff_ffff;
			var bucketIndex = hash % _buckets.Length;
			int targetEntryIndex;

			for (int entryIndex = _buckets[bucketIndex]; entryIndex >= 0; entryIndex = _entries[entryIndex].Next)
			{
				if (_entries[entryIndex].Hash == hash)
				{
					if (add)
					{
						throw new Exception("Same Key");
					}

					_entries[entryIndex].Value = value;
					return;
				}
			}

			if (_holeCount > 0)
			{
				targetEntryIndex = _holeIndex;
				_holeIndex = _entries[targetEntryIndex].Next;
				--_holeCount;
			}
			else
			{
				if (_count == _entries.Length)
				{
					Resize();
					bucketIndex = hash % _buckets.Length;
				}

				targetEntryIndex = _count++;
			}

			_entries[targetEntryIndex].Key = key;
			_entries[targetEntryIndex].Hash = hash;
			_entries[targetEntryIndex].Value = value;
			_entries[targetEntryIndex].Next = _buckets[bucketIndex];
			_buckets[bucketIndex] = targetEntryIndex;
		}

		private void Resize()
		{
			var newSize = PrimeHelper.GetPrime(_buckets.Length);
			var newBuckets = new int[newSize];
			var newEntries = new Entry[newSize];
			Array.Copy(_entries, newEntries, _count);
			for (int i = newSize - 1; i >= 0; --i)
			{
				newBuckets[i] = -1;
			}

			for (int i = 0; i < _count; ++i)
			{
				if (newEntries[i].Hash < 0)
				{
					continue;
				}

				var newBucketIndex = newEntries[i].Hash % newSize;
				newEntries[i].Next = newBuckets[newBucketIndex];
				newBuckets[newBucketIndex] = i;
			}

			_buckets = newBuckets;
			_entries = newEntries;
		}

		private int FindEntry(TKey key)
		{
			var hash = key.GetHashCode() & 0x7fff_ffff;
			for (int i = _buckets[hash % _buckets.Length]; i >= 0; i = _entries[i].Next)
			{
				if (_entries[i].Hash == hash)
				{
					return i;
				}
			}

			return -1;
		}

		public IEnumerator<TValue> GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}

		[Serializable]
		public struct Enumerator : IEnumerator<TValue>, System.Collections.IEnumerator
		{
			private DetDictionary<TKey, TValue> _dictionary;
			private int _index;
			private TValue _currentValue;

			internal Enumerator(DetDictionary<TKey, TValue> dictionary)
			{
				this._dictionary = dictionary;
				_index = 0;
				_currentValue = default;
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				while ((uint)_index < (uint)_dictionary._count)
				{
					if (_dictionary._entries[_index].Hash >= 0)
					{
						_currentValue = _dictionary._entries[_index].Value;
						_index++;
						return true;
					}
					_index++;
				}

				_index = _dictionary._count + 1;
				_currentValue = default;
				return false;
			}

			public TValue Current
			{
				get
				{
					return _currentValue;
				}
			}

			Object System.Collections.IEnumerator.Current
			{
				get
				{
					if (_index == 0 || (_index == _dictionary._count + 1))
					{
						throw new Exception();
					}

					return _currentValue;
				}
			}

			void System.Collections.IEnumerator.Reset()
			{
				_index = 0;
				_currentValue = default(TValue);
			}
		}
	}
}
