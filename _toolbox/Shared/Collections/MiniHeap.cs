
namespace _t.Shared.Collections
{
    public class MinHeap<TKey>
    {
        private readonly List<(TKey Key, float Priority)> _heap = new();
        private readonly Dictionary<TKey, int> _indexMap = new();
        private readonly IComparer<float> _comparer;

        public MinHeap()
        {
            _comparer = Comparer<float>.Default;
        }

        public int Count => _heap.Count;

        public bool Contains(TKey key) => _indexMap.ContainsKey(key);

        public void Insert(TKey key, float priority)
        {
            if (_indexMap.ContainsKey(key))
                throw new InvalidOperationException($"Key '{key}' already exists in heap.");

            _heap.Add((key, priority));
            int index = _heap.Count - 1;
            _indexMap[key] = index;
            HeapifyUp(index);
        }

        public void UpdatePriority(TKey key, float newPriority)
        {
            if (!_indexMap.TryGetValue(key, out int index))
                throw new KeyNotFoundException($"Key '{key}' not found in heap.");

            float oldPriority = _heap[index].Priority;
            _heap[index] = (key, newPriority);

            if (_comparer.Compare(newPriority, oldPriority) < 0)
                HeapifyUp(index);
            else
                HeapifyDown(index);
        }

        public TKey PeekMin()
        {
            if (_heap.Count == 0)
                throw new InvalidOperationException("Heap is empty.");
            return _heap[0].Key;
        }

        public TKey ExtractMin()
        {
            if (_heap.Count == 0)
                throw new InvalidOperationException("Heap is empty.");

            TKey minKey = _heap[0].Key;
            var last = _heap[^1];
            _heap[0] = last;
            _indexMap[last.Key] = 0;

            _heap.RemoveAt(_heap.Count - 1);
            _indexMap.Remove(minKey);

            if (_heap.Count > 0)
                HeapifyDown(0);

            return minKey;
        }

        public void Remove(TKey key)
        {
            if (!_indexMap.TryGetValue(key, out int index))
                throw new KeyNotFoundException($"Key '{key}' not found in heap.");

            int lastIndex = _heap.Count - 1;
            if (index != lastIndex)
            {
                var last = _heap[lastIndex];
                _heap[index] = last;
                _indexMap[last.Key] = index;
            }

            _heap.RemoveAt(lastIndex);
            _indexMap.Remove(key);

            if (index < _heap.Count)
            {
                int parent = (index - 1) / 2;
                if (index > 0 && _comparer.Compare(_heap[index].Priority, _heap[parent].Priority) < 0)
                    HeapifyUp(index);
                else
                    HeapifyDown(index);
            }
        }

        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (_comparer.Compare(_heap[index].Priority, _heap[parent].Priority) >= 0)
                    break;

                Swap(index, parent);
                index = parent;
            }
        }

        private void HeapifyDown(int index)
        {
            int lastIndex = _heap.Count - 1;

            while (true)
            {
                int left = 2 * index + 1;
                int right = 2 * index + 2;
                int smallest = index;

                if (left <= lastIndex && _comparer.Compare(_heap[left].Priority, _heap[smallest].Priority) < 0)
                    smallest = left;
                if (right <= lastIndex && _comparer.Compare(_heap[right].Priority, _heap[smallest].Priority) < 0)
                    smallest = right;

                if (smallest == index)
                    break;

                Swap(index, smallest);
                index = smallest;
            }
        }

        private void Swap(int a, int b)
        {
            (_heap[a], _heap[b]) = (_heap[b], _heap[a]);
            _indexMap[_heap[a].Key] = a;
            _indexMap[_heap[b].Key] = b;
        }
    }
}
