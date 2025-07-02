
namespace _t.Shared.Caching
{
    public class DoublyLinkedList<T>
    {
        public Node<T> Head { get; private set; }
        public Node<T> Tail { get; private set; }

        public void AddFirst(Node<T> node)
        {
            node.Next = Head;
            node.Prev = null;

            if (Head != null)
                Head.Prev = node;

            Head = node;

            if (Tail == null)
                Tail = Head;
        }

        public void MoveToFront(Node<T> node)
        {
            if (node == Head) return;
            Remove(node);
            AddFirst(node);
        }

        public void Remove(Node<T> node)
        {
            if (node.Prev != null)
                node.Prev.Next = node.Next;
            else
                Head = node.Next;

            if (node.Next != null)
                node.Next.Prev = node.Prev;
            else
                Tail = node.Prev;

            node.Prev = node.Next = null;
        }

        public Node<T> RemoveLast()
        {
            if (Tail == null) return null;

            var last = Tail;
            Remove(last);
            return last;
        }

        public void Print()
        {
            var current = Head;
            while (current != null)
            {
                Console.Write($"[{current.Value}] <-> ");
                current = current.Next;
            }
            Console.WriteLine("null");
        }
    }
}
