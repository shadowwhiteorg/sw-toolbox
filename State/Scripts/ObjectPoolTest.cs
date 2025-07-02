using System;
using UnityEngine;
using _t.Shared.Pooling;
using _t.Unity.ObjectPool;

namespace _t.Unity
{
    public class ObjectPoolTest : MonoBehaviour
    {
        private ObjectPool<int, DamageEvent> _testPool = new ObjectPool<int, DamageEvent>(
            capacity: 20,
            ttl: TimeSpan.FromSeconds(10),
            keyGenerator: () => 0, // dummy key for single-type pool
            factory: _ => new DamageEvent(),
            onReturn: e => e.Clear()
            );
        
        void Start()
        {
            var eventPool = new ObjectPool<int,DamageEvent>(
                capacity: 20,
                ttl: TimeSpan.FromSeconds(10),
                keyGenerator: () => 0, // dummy key for single-type pool
                factory: _ => new DamageEvent(),
                onReturn: e => e.Clear()
            );
        }
    }
}

