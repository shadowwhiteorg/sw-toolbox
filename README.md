# Project Overview: Generic Data-Aware Tools with Unity Integration

## What's this all about?

This repo gathers a bunch of reusable, data-driven tools that play nicely with Unity. We rely on custom data structures (like `DoublyLinkedList`, `MinHeap`, and `CustomDictionary`) to keep caching, pooling, and batching blazing fast. Everything's cleanly split up so you can use the core logic anywhere and drop the Unity pieces right into a scene:

- **/Shared** – platform-neutral code and data structures
- **/Assets/Scripts/** – Unity wrappers for runtime MonoBehaviours

These tools are made to run in a console app for easy testing, then plug straight into Unity for games or other projects. They're flexible enough for object pools, telemetry batching, and other memory-conscious systems.

---

## Unified Data Structure Usage Table

### Core structures and Unity context

| Data Structure | Purpose | Used In | Unity Integration & Usage |
|---------------|---------|---------|---------------------------|
| **DoublyLinkedList<T>** | Quickly reorders items for LRU eviction | `Cache`, `LRUCache`, `LFUCache` | Used in `ObjectPool` to track pooled objects |
| **Node<T>** | Linked list node holding value and pointers | `DoublyLinkedList<T>` | Internal piece used indirectly via `ObjectPool` and `Cache` |
| **CustomDictionary<TKey, TValue>** | Hash table with separate chaining | `BatchDispatcher`, `LFUCache`, `Cache` | Groups events by type in `TelemetryBatchUploader` |
| **MinHeap<TKey>** | Keeps ascending-priority order (for deadlines, TTLs) | `BatchDispatcher` | Schedules flush deadlines in `TelemetryBatchUploader` |
| **Cache<K, V>** | Hybrid cache with TTL + LRU | `ObjectPool<TKey, TObject>` | Drives Unity-side pooling (`DamageEvent`, `GameObjectPool`) |
| **LRUCache<K, V>** | Evicts least recently used item (no TTL) | Alternative to `Cache` | Works with `ObjectPool`, but usually replaced by `Cache` |
| **LFUCache<K, V>** | Evicts least frequently used items with TTL support | `Program.cs` testbed | Not yet used in Unity but ready for it |
| **BatchDispatcher<TKey, TItem>** | Groups items and dispatches when batch size or time limit hits | `TelemetryBatchUploader` | Core engine for batching logs/events in Unity |
| **ICache<K, V>** | Interface for any cache strategy | `ObjectPool`, `CacheRegistry` | Lets you plug different caching behaviors into Unity pools |
| **ObjectPool<TKey, TObject>** | Generic pooling system with TTL and LRU | `ObjectPoolTest`, `DamageEvent` | Reuses any type (not just `GameObject`) and auto-evicts |

---

## Example Unity Usage Mapping

| Unity Feature/Script | Relies on These Structures |
|----------------------|---------------------------|
| `TelemetryBatchUploader` | `BatchDispatcher`, `MinHeap`, `CustomDictionary` |
| `ObjectPoolTest`, `DamageEvent` | `ObjectPool`, `Cache`, `DoublyLinkedList`, `ICache` |
| `PoolTest` in console app | `DoublyLinkedList`, simple memory reuse |
