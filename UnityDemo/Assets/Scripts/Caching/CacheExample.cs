using System;
using UnityEngine;
using _t.Shared.Caching;

namespace _t.Unity.Caching
{
    public class CacheExample : MonoBehaviour
    {
        [SerializeField]
        private CacheSettings CacheConfig;

        private ICache<string, string> _cache;

        private void Awake()
        {
            int capacity = CacheConfig != null ? CacheConfig.Capacity : 10;
            float ttl = CacheConfig != null ? CacheConfig.TtlSeconds : 30f;

            _cache = new Cache<string, string>(capacity, TimeSpan.FromSeconds(ttl));
            CacheRegistry.Register("example", () => _cache);
        }
    }
}
