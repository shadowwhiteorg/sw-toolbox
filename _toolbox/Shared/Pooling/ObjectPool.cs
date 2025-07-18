using System;
using System.Collections.Generic;

namespace _t.Shared.Pooling
{
    public class ObjectPool<TKey, TObject> : IPool<TObject>
    {
        private readonly int _capacity;
        private readonly TimeSpan _ttl;
        private readonly Func<TKey> _keyGenerator;
        private readonly Func<TKey, TObject> _factory;
        private readonly Action<TObject> _onReturn;
        private readonly Queue<(TObject Obj, DateTime Expiry)> _available = new();

        public ObjectPool(
            int capacity,
            TimeSpan ttl,
            Func<TKey> keyGenerator,
            Func<TKey, TObject> factory,
            Action<TObject> onReturn = null)
        {
            _capacity = capacity;
            _ttl = ttl;
            _keyGenerator = keyGenerator;
            _factory = factory;
            _onReturn = onReturn;
        }

        public TObject Get()
        {
            while (_available.Count > 0)
            {
                var (obj, expiry) = _available.Dequeue();
                if (expiry > DateTime.UtcNow)
                    return obj;
            }

            var key = _keyGenerator();
            return _factory(key);
        }

        public void Return(TObject item)
        {
            _onReturn?.Invoke(item);

            if (_available.Count >= _capacity)
                return;

            var expiry = DateTime.UtcNow + _ttl;
            _available.Enqueue((item, expiry));
        }

        public void Clear()
        {
            _available.Clear();
        }

        public int Count => _available.Count;
    }
}
