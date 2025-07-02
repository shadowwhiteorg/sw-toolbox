using _t.Playground;
using _t.Shared.Caching;
using _t.Shared.LFU;
using _t.Shared.Batching;

LinkedListTest.Run();
PoolTest poolTest = new();
PoolTest.CreateInstance();


ICache<string, string> cache = new LFUCache<string, string>(capacity: 3, ttl: TimeSpan.FromSeconds(3));

cache.Put("a", "apple");
// Thread.Sleep(1000); // uncomment for test
Console.WriteLine(cache.TryGet("a", out var value)); // false (expired)