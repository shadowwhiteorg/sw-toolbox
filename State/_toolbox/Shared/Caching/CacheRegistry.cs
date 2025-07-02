
namespace _t.Shared.Caching
{
    public static class CacheRegistry
    {
        private static readonly Dictionary<string, object> _factories = new();

        public static void Register<K, V>(string name, Func<ICache<K, V>> factory)
        {
            _factories[name] = factory;
        }

        public static ICache<K, V> Resolve<K, V>(string name)
        {
            if (_factories.TryGetValue(name, out var factoryObj) && factoryObj is Func<ICache<K, V>> factory)
                return factory();

            throw new KeyNotFoundException($"No cache registered under name: {name}");
        }

        public static void Clear() => _factories.Clear();
    }
}