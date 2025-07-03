
namespace _t.Unity.ObjectPool
{
    public class DamageEvent
    {
        public int Damage;
        public string Source;
        public float Time;
        public void Clear()
        {
            Damage = 0;
            Source = null;
            Time = 0;
        }
    }
}