using System;
using System.Collections.Generic;

namespace _t.Shared.Batching
{
    /// <summary>
    /// Dispatches items grouped by a key, either when a group's
    /// batch size is reached, or when its dispatch deadline arrives.
    /// </summary>
    public interface IBatchDispatcher<TKey, TItem>
    {
        /// <summary>Enqueue an item under the given group key.</summary>
        void Enqueue(TKey key, TItem item);

        /// <summary>
        /// Should be called periodically (e.g. each frame or tick).
        /// Flushes any groups whose deadline has passed.
        /// </summary>
        void Tick();

        /// <summary>Immediately flushes all pending groups.</summary>
        void FlushAll();

        /// <summary>Total items pending across all groups.</summary>
        int PendingCount { get; }
    }
}