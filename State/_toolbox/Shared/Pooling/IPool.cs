namespace _t.Shared.Pooling
{
    public interface IPool<T>
    {
        T Get();
        void Return(T item);
        void Clear();
        int Count { get; }
    }
}