using UnityEngine;

namespace _t.Unity.Caching
{
    [CreateAssetMenu(fileName = "CacheSettings", menuName = "_t/Cache Settings")]
    public class CacheSettings : ScriptableObject
    {
        [Min(1)]
        public int Capacity = 10;

        [Tooltip("Time to live in seconds before an entry expires")]
        [Min(0f)]
        public float TtlSeconds = 30f;
    }
}
