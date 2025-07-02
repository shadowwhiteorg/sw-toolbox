using System;
using System.Collections.Generic;
using _t.Shared.Caching;

namespace _t.Shared.LRU
{
    public class LRUCache<K, V> : ICache<K, V>
    {
        private readonly int _capacity;
        private readonly Dictionary<K, Node<CacheItem>> _map;
        private readonly DoublyLinkedList<CacheItem> _list;

        private class CacheItem
        {
            public K Key { get; }
            public V Value { get; set; }

            public CacheItem(K key, V value)
            {
                Key = key;
                Value = value;
            }
        }

        public LRUCache(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException("Capacity must be greater than 0.");
            _capacity = capacity;
            _map = new Dictionary<K, Node<CacheItem>>();
            _list = new DoublyLinkedList<CacheItem>();
        }

        public V Get(K key)
        {
            if (!_map.TryGetValue(key, out var node))
                throw new KeyNotFoundException($"Key '{key}' not found.");
            _list.MoveToFront(node);
            return node.Value.Value;
        }

        public bool TryGet(K key, out V value)
        {
            if (_map.TryGetValue(key, out var node))
            {
                _list.MoveToFront(node);
                value = node.Value.Value;
                return true;
            }

            value = default;
            return false;
        }

        public void Put(K key, V value)
        {
            if (_map.TryGetValue(key, out var existingNode))
            {
                existingNode.Value.Value = value;
                _list.MoveToFront(existingNode);
                return;
            }

            if (_map.Count >= _capacity)
            {
                var lru = _list.RemoveLast();
                if (lru != null)
                    _map.Remove(lru.Value.Key);
            }

            var item = new CacheItem(key, value);
            var node = new Node<CacheItem>(item);
            _list.AddFirst(node);
            _map[key] = node;
        }

        public bool ContainsKey(K key) => _map.ContainsKey(key);

        public void Clear()
        {
            _map.Clear();
            while (_list.RemoveLast() != null) { }
        }

        public int Count => _map.Count;
    }
}
