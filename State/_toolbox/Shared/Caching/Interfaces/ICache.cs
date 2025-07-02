namespace _t.Shared.Caching
{
    public interface ICache<K, V>
    {
        V Get(K key);                          // throws if not found
        bool TryGet(K key, out V value);       // safer version
        void Put(K key, V value);              // insert or update
        bool ContainsKey(K key);
        void Clear();
        int Count { get; }
    }
}