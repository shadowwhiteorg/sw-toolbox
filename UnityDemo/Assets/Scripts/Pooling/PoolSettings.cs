using UnityEngine;

namespace _t.Unity.ObjectPool
{
    [CreateAssetMenu(fileName = "PoolSettings", menuName = "_t/Pool Settings")]
    public class PoolSettings : ScriptableObject
    {
        [Min(1)]
        public int Capacity = 10;

        [Tooltip("Time to live in seconds for returned objects before reuse")] 
        [Min(0f)]
        public float TtlSeconds = 10f;
    }
}