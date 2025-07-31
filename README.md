#  Project Report: Generic Data-Aware Tools with Unity Integration

##  Overview

This project implements a suite of highly generic, reusable data-driven tools that integrate efficiently with Unity. It uses custom data structures (e.g., `DoublyLinkedList`, `MinHeap`, `CustomDictionary`) to build high-performance caching, pooling, and batching systems. The codebase is cleanly divided into:

- **/Shared**: Platform-independent core logic and data structures.
- **/Assets/Scripts/**: Unity-specific wrappers for runtime MonoBehaviours.

The goal is to develop tools that are testable in console apps, portable to Unity, and extensible across domains like pooling, telemetry batching, and memory-aware gameplay systems.

---

##  Unified Data Structure Usage Table

### Core Structures and Unity Context

| Data Structure                       | Purpose                                                                 | Used In                                 | Unity Integration & Usage                                    |
|--------------------------------------|-------------------------------------------------------------------------|------------------------------------------|--------------------------------------------------------------|
| **DoublyLinkedList<T>**              | Enables fast LRU eviction and reordering                               | `Cache`, `LRUCache`, `LFUCache`          | Used in `ObjectPool` to manage LRU of pooled objects         |
| **Node<T>**                          | Linked list node, holds value + next/prev                              | `DoublyLinkedList<T>`                    | Internal structure, indirectly used in Unity via `ObjectPool` and `Cache` |
| **CustomDictionary<TKey, TValue>**   | Hash table with separate chaining for key/value storage                | `BatchDispatcher`, `LFUCache`, `Cache`   | Used in `TelemetryBatchUploader` to group events by type     |
| **MinHeap<TKey>**                    | Maintains ascending-priority order (e.g., deadlines, TTLs)             | `BatchDispatcher`                        | Schedules flush deadlines for event groups in `TelemetryBatchUploader` |
| **Cache<K, V>**                      | TTL + LRU eviction hybrid cache                                        | `ObjectPool<TKey, TObject>`              | Drives core reuse system in Unity-side pooling (`DamageEvent`, `GameObjectPool`) |
| **LRUCache<K, V>**                   | Evicts least recently used items (no TTL)                              | Alternative to `Cache`                   | Usable with `ObjectPool`, but superseded by `Cache` for TTL+LRU |
| **LFUCache<K, V>**                   | Evicts least frequently used + TTL support                             | `Program.cs` testbed                     | Not yet wired into Unity, but prepared for frequency-aware tools |
| **BatchDispatcher<TKey, TItem>**     | Groups items, dispatches when batch size or time exceeded              | `TelemetryBatchUploader`                 | Primary engine for batching logs/events in Unity             |
| **ICache<K, V>**                     | Interface for any cache strategy                                       | `ObjectPool`, `CacheRegistry`            | Enables injection of caching behavior in pooled Unity systems |
| **ObjectPool<TKey, TObject>**        | Pooling system with TTL and LRU, generic and cache-backed              | `ObjectPoolTest`, `DamageEvent`          | Reuses any type (not just `GameObject`), TTL ensures auto-eviction in Unity |

---

##  Example Unity Usage Mapping

| Unity Feature/Script            | Relies on These Structures                                |
|---------------------------------|------------------------------------------------------------|
| `TelemetryBatchUploader`        | `BatchDispatcher`, `MinHeap`, `CustomDictionary`           |
| `ObjectPoolTest`, `DamageEvent` | `ObjectPool`, `Cache`, `DoublyLinkedList`, `ICache`        |
| `PoolTest` in console app       | `DoublyLinkedList`, basic memory reuse                     |