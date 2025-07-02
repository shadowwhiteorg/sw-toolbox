using System;
using System.Collections.Generic;
using _t.Shared.Caching;

namespace _t.Shared.LFU
{
    public class LFUCache<K, V> : ICache<K, V>
    {
        private readonly int _capacity;
        private readonly TimeSpan _ttl;
        private readonly Dictionary<K, Node<CacheItem>> _nodeMap;
        private readonly Dictionary<int, DoublyLinkedList<CacheItem>> _frequencyMap;
        private readonly Dictionary<K, DateTime> _expiryMap;
        private int _minFrequency;

        private class CacheItem
        {
            public K Key { get; }
            public V Value { get; set; }
            public int Frequency { get; set; }

            public CacheItem(K key, V value)
            {
                Key = key;
                Value = value;
                Frequency = 1;
            }
        }

        public LFUCache(int capacity, TimeSpan ttl)
        {
            if (capacity <= 0)
                throw new ArgumentException("Capacity must be greater than 0.");
            _capacity = capacity;
            _ttl = ttl;
            _nodeMap = new Dictionary<K, Node<CacheItem>>();
            _frequencyMap = new Dictionary<int, DoublyLinkedList<CacheItem>>();
            _expiryMap = new Dictionary<K, DateTime>();
        }

        public V Get(K key)
        {
            if (IsExpired(key))
                throw new KeyNotFoundException($"Key '{key}' expired.");

            var node = _nodeMap[key];
            Touch(node);
            return node.Value.Value;
        }

        public bool TryGet(K key, out V value)
        {
            if (IsExpired(key))
            {
                Remove(key);
                value = default;
                return false;
            }

            var node = _nodeMap[key];
            Touch(node);
            value = node.Value.Value;
            return true;
        }

        public void Put(K key, V value)
        {
            if (_nodeMap.TryGetValue(key, out var existingNode))
            {
                existingNode.Value.Value = value;
                _expiryMap[key] = DateTime.UtcNow + _ttl;
                Touch(existingNode);
                return;
            }

            if (_nodeMap.Count >= _capacity)
                Evict();

            var item = new CacheItem(key, value);
            var newNode = new Node<CacheItem>(item);
            _nodeMap[key] = newNode;
            _expiryMap[key] = DateTime.UtcNow + _ttl;

            AddToFrequencyList(1, newNode);
            _minFrequency = 1;
        }

        public bool ContainsKey(K key)
        {
            if (IsExpired(key))
            {
                Remove(key);
                return false;
            }

            return _nodeMap.ContainsKey(key);
        }

        public void Clear()
        {
            _nodeMap.Clear();
            _frequencyMap.Clear();
            _expiryMap.Clear();
            _minFrequency = 0;
        }

        public int Count => _nodeMap.Count;

        private void Touch(Node<CacheItem> node)
        {
            var oldFreq = node.Value.Frequency;
            node.Value.Frequency++;

            _frequencyMap[oldFreq].Remove(node);
            if (_frequencyMap[oldFreq].Head == null && _minFrequency == oldFreq)
                _minFrequency++;

            AddToFrequencyList(node.Value.Frequency, node);
        }

        private void AddToFrequencyList(int frequency, Node<CacheItem> node)
        {
            if (!_frequencyMap.TryGetValue(frequency, out var list))
            {
                list = new DoublyLinkedList<CacheItem>();
                _frequencyMap[frequency] = list;
            }

            list.AddFirst(node);
        }

        private void Evict()
        {
            if (!_frequencyMap.TryGetValue(_minFrequency, out var list))
                throw new InvalidOperationException("Inconsistent state in frequency map.");

            var nodeToRemove = list.RemoveLast();
            if (nodeToRemove != null)
                Remove(nodeToRemove.Value.Key);
        }

        private bool IsExpired(K key)
        {
            return _expiryMap.TryGetValue(key, out var expiry) && expiry < DateTime.UtcNow;
        }

        private void Remove(K key)
        {
            _expiryMap.Remove(key);

            if (_nodeMap.TryGetValue(key, out var node))
            {
                _frequencyMap[node.Value.Frequency].Remove(node);
                _nodeMap.Remove(key);
            }
        }
    }
}
