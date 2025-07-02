

namespace _t.Shared.Batching
{
    public interface IBatchDispatcher<T>
    {
        void Enqueue(T item);
        void Tick(); // should be called periodically (manually or from Unity)
        void Flush(); // manual trigger
        int PendingCount { get; }
    }
}