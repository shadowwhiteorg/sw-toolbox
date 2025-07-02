
namespace _t.Shared.Caching
{
    public static class LinkedListTest
    {
        public static void Run()
        {
            var list = new DoublyLinkedList<string>();
        
            var a = new Node<string>("A");
            var b = new Node<string>("B");
            var c = new Node<string>("C");
            var d = new Node<string>("D");
        
            Console.WriteLine("Initial Add:");
            list.AddFirst(a);
            list.AddFirst(b);
            list.AddFirst(c);
            list.Print(); // C <-> B <-> A
        
            Console.WriteLine(" Move A to front:");
            list.MoveToFront(a);
            list.Print(); // A <-> C <-> B
        
            Console.WriteLine(" Remove C:");
            list.Remove(c);
            list.Print(); // A <-> B
        
            Console.WriteLine(" Add D to front:");
            list.AddFirst(d);
            list.Print(); // D <-> A <-> B
        
            Console.WriteLine(" Remove Last:");
            var last = list.RemoveLast();
            Console.WriteLine($"Removed: {last.Value}");
            list.Print(); // D <-> A
        }
    }
}