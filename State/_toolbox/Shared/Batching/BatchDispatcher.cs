using System;
using System.Collections.Generic;
using _t.Shared.Collections;

namespace _t.Shared.Batching
{

    public class BatchDispatcher<TKey, TItem> : IBatchDispatcher<TKey, TItem>
    {
        private readonly CustomDictionary<TKey, List<TItem>> _groups
            = new CustomDictionary<TKey, List<TItem>>();
        private readonly MinHeap<TKey> _deadlineHeap = new MinHeap<TKey>();
        private readonly Dictionary<TKey, float> _deadlines = new Dictionary<TKey, float>();

        private readonly int _batchSize;
        private readonly float _dispatchIntervalSeconds;
        private readonly Action<TKey, List<TItem>> _onDispatch;

        private static readonly DateTime UnixEpoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static float NowSeconds() =>
            (float)(DateTime.UtcNow - UnixEpoch).TotalSeconds;

        public BatchDispatcher(
            int batchSize,
            TimeSpan dispatchInterval,
            Action<TKey, List<TItem>> onDispatch)
        {
            if (batchSize <= 0) throw new ArgumentException("batchSize must be > 0");
            _batchSize = batchSize;
            _dispatchIntervalSeconds = (float)dispatchInterval.TotalSeconds;
            _onDispatch = onDispatch ?? throw new ArgumentNullException(nameof(onDispatch));
        }

        public int PendingCount
        {
            get
            {
                int sum = 0;
                foreach (var key in _deadlines.Keys)
                {
                    if (_groups.TryGetValue(key, out var lst))
                        sum += lst.Count;
                }
                return sum;
            }
        }

        public void Enqueue(TKey key, TItem item)
        {
            if (!_groups.TryGetValue(key, out var list))
            {
                list = new List<TItem>();
                _groups.Add(key, list);
                ScheduleDeadline(key);
            }

            list.Add(item);
            if (list.Count >= _batchSize)
                FlushGroup(key);
        }

        public void Tick()
        {
            float now = NowSeconds();
            while (_deadlineHeap.Count > 0)
            {
                var nextKey = _deadlineHeap.PeekMin();
                if (_deadlines[nextKey] > now) break;
                FlushGroup(nextKey);
            }
        }

        public void FlushAll()
        {
            var keys = new List<TKey>(_deadlines.Keys);
            foreach (var key in keys)
                FlushGroup(key);
        }

        private void ScheduleDeadline(TKey key)
        {
            float deadline = NowSeconds() + _dispatchIntervalSeconds;
            _deadlines[key] = deadline;

            if (_deadlineHeap.Contains(key))
                _deadlineHeap.UpdatePriority(key, deadline);
            else
                _deadlineHeap.Insert(key, deadline);
        }

        private void FlushGroup(TKey key)
        {
            if (!_groups.TryGetValue(key, out var list) || list.Count == 0)
                return;

            var batch = new List<TItem>(list);
            _onDispatch(key, batch);

            _groups.Remove(key);
            _deadlines.Remove(key);
            if (_deadlineHeap.Contains(key))
                _deadlineHeap.ExtractMin();
        }
    }
}
