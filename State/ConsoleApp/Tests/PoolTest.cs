using _t.Shared.Caching;

namespace _t.Playground
{
    public class PoolTest
    {
        public static PoolTest CreateInstance()
        {
            Console.WriteLine($"Pooltest ctreated.");
            return new PoolTest();
        }
        
        public DoublyLinkedList<float> DoublyLinkedList = new();
    }
}
