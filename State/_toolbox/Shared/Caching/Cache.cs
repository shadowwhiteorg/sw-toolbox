namespace _t.Shared.Caching
{
    public class Cache<K, V> : ICache<K, V>
    {
        private readonly int _capacity;
        private readonly TimeSpan _ttl;
        private readonly Dictionary<K, Node<CacheItem>> _map;
        private readonly DoublyLinkedList<CacheItem> _lruList;

        private class CacheItem
        {
            public K Key { get; }
            public V Value { get; set; }
            public DateTime ExpiryTime { get; set; }

            public CacheItem(K key, V value, DateTime expiryTime)
            {
                Key = key;
                Value = value;
                ExpiryTime = expiryTime;
            }
        }

        public Cache(int capacity, TimeSpan ttl)
        {
            if (capacity <= 0)
                throw new ArgumentException("Capacity must be greater than 0.");
            _capacity = capacity;
            _ttl = ttl;
            _map = new Dictionary<K, Node<CacheItem>>();
            _lruList = new DoublyLinkedList<CacheItem>();
        }

        public V Get(K key)
        {
            if (IsExpired(key))
                throw new KeyNotFoundException($"Key '{key}' expired.");

            var node = _map[key];
            _lruList.MoveToFront(node);
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

            var node = _map[key];
            _lruList.MoveToFront(node);
            value = node.Value.Value;
            return true;
        }

        public void Put(K key, V value)
        {
            if (_map.TryGetValue(key, out var node))
            {
                node.Value.Value = value;
                node.Value.ExpiryTime = DateTime.UtcNow + _ttl;
                _lruList.MoveToFront(node);
                return;
            }

            if (_map.Count >= _capacity)
                Evict();

            var expiry = DateTime.UtcNow + _ttl;
            var item = new CacheItem(key, value, expiry);
            var newNode = new Node<CacheItem>(item);
            _lruList.AddFirst(newNode);
            _map[key] = newNode;
        }

        public bool ContainsKey(K key)
        {
            if (IsExpired(key))
            {
                Remove(key);
                return false;
            }

            return _map.ContainsKey(key);
        }

        public void Clear()
        {
            _map.Clear();
            while (_lruList.RemoveLast() != null) { }
        }

        public int Count => _map.Count;

        private void Evict()
        {
            foreach (var kvp in new List<K>(_map.Keys))
            {
                if (IsExpired(kvp))
                    Remove(kvp);
            }

            if (_map.Count < _capacity) return;

            var node = _lruList.RemoveLast();
            if (node != null)
                _map.Remove(node.Value.Key);
        }

        private bool IsExpired(K key)
        {
            return _map.TryGetValue(key, out var node)
                && node.Value.ExpiryTime < DateTime.UtcNow;
        }

        private void Remove(K key)
        {
            if (_map.TryGetValue(key, out var node))
            {
                _lruList.Remove(node);
                _map.Remove(key);
            }
        }
    }
}
