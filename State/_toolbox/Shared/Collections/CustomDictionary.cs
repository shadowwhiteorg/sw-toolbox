using System;
using System.Collections;
using System.Collections.Generic;

namespace _t.Shared.Collections
{
    public class CustomDictionary<TKey, TValue>
    {
        private const int DefaultCapacity = 16;
        private LinkedList<Entry>[] _buckets;
        private int _count;

        private class Entry
        {
            public TKey Key;
            public TValue Value;

            public Entry(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }

        public CustomDictionary(int capacity = DefaultCapacity)
        {
            _buckets = new LinkedList<Entry>[capacity];
            _count = 0;
        }

        private int GetIndex(TKey key)
        {
            return Math.Abs(key.GetHashCode()) % _buckets.Length;
        }

        public void Add(TKey key, TValue value)
        {
            int index = GetIndex(key);

            _buckets[index] ??= new LinkedList<Entry>();

            foreach (var entry in _buckets[index])
            {
                if (EqualityComparer<TKey>.Default.Equals(entry.Key, key))
                    throw new ArgumentException($"Duplicate key: {key}");
            }

            _buckets[index].AddLast(new Entry(key, value));
            _count++;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = GetIndex(key);
            var bucket = _buckets[index];

            if (bucket != null)
            {
                foreach (var entry in bucket)
                {
                    if (EqualityComparer<TKey>.Default.Equals(entry.Key, key))
                    {
                        value = entry.Value;
                        return true;
                    }
                }
            }

            value = default;
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return TryGetValue(key, out _);
        }

        public bool Remove(TKey key)
        {
            int index = GetIndex(key);
            var bucket = _buckets[index];

            if (bucket != null)
            {
                var node = bucket.First;
                while (node != null)
                {
                    if (EqualityComparer<TKey>.Default.Equals(node.Value.Key, key))
                    {
                        bucket.Remove(node);
                        _count--;
                        return true;
                    }
                    node = node.Next;
                }
            }

            return false;
        }

        public TValue this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out var value))
                    return value;
                throw new KeyNotFoundException($"Key not found: {key}");
            }
            set
            {
                if (ContainsKey(key))
                {
                    int index = GetIndex(key);
                    foreach (var entry in _buckets[index])
                    {
                        if (EqualityComparer<TKey>.Default.Equals(entry.Key, key))
                        {
                            entry.Value = value;
                            return;
                        }
                    }
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        public void Clear()
        {
            _buckets = new LinkedList<Entry>[_buckets.Length];
            _count = 0;
        }

        public int Count => _count;
    }
}
