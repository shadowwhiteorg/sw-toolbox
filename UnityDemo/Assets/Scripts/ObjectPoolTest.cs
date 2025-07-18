using System;
using UnityEngine;
using _t.Shared.Pooling;
using _t.Unity.ObjectPool;

namespace _t.Unity
{
    public class ObjectPoolTest : MonoBehaviour
    {
        [SerializeField]
        private PoolSettings PoolConfig;

        private ObjectPool<int, DamageEvent> _testPool;

        private void Awake()
        {
            int capacity = PoolConfig != null ? PoolConfig.Capacity : 20;
            float ttlSeconds = PoolConfig != null ? PoolConfig.TtlSeconds : 10f;

            _testPool = new ObjectPool<int, DamageEvent>(
                capacity: capacity,
                ttl: TimeSpan.FromSeconds(ttlSeconds),
                keyGenerator: () => 0,
                factory: _ => new DamageEvent(),
                onReturn: e => e.Clear()
            );
        }
    }
}
