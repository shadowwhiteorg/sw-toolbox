using _t.Shared.Caching;

namespace _t.Shared.Pooling
{
    public class ObjectPool<TKey, TObject> : IPool<TObject>
    {
        private readonly ICache<TKey, TObject> _cache;
        private readonly Func<TKey> _keyGenerator;
        private readonly Func<TKey, TObject> _factory;
        private readonly Action<TObject> _onReturn;

        public ObjectPool(
            int capacity,
            TimeSpan ttl,
            Func<TKey> keyGenerator,
            Func<TKey, TObject> factory,
            Action<TObject> onReturn = null)
        {
            _cache = new Cache<TKey, TObject>(capacity, ttl);
            _keyGenerator = keyGenerator;
            _factory = factory;
            _onReturn = onReturn;
        }

        public TObject Get()
        {
            var key = _keyGenerator();

            if (_cache.TryGet(key, out var obj))
                return obj;

            obj = _factory(key);
            _cache.Put(key, obj);
            return obj;
        }

        public void Return(TObject item)
        {
            _onReturn?.Invoke(item);
        }

        public void Clear()
        {
            _cache.Clear();
        }

        public int Count => _cache.Count;
    }
}