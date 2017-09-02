using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Tydbits.LruCache.Tests
{
    [TestFixture]
    internal class LruCacheTests
    {
        private const int CacheSize = 5;
        private LruCache _cache;

        [SetUp]
        public void SetUp()
        {
            _cache = new LruCache(CacheSize);
        }

        [DatapointSource]
        public IEnumerable<KeyValuePair<string, object>[]> Data
        {
            get
            {
                yield return GenerateData(CacheSize / 2).ToArray();
                yield return GenerateData(CacheSize).ToArray();
                yield return GenerateData(CacheSize * 2).ToArray();
            }
        }

        [Theory]
        public void When_DataSize_IsLessOrEqualTo_CacheSize(KeyValuePair<string, object>[] data)
        {
            Assume.That(data.Length <= CacheSize);

            foreach (var kv in data)
                _cache.Set(kv.Key, kv.Value);

            foreach (var kv in data)
                Assert.That(_cache.Get(kv.Key), Is.EqualTo(kv.Value), "Then all items should be present");
        }

        [Theory]
        public void When_DataSize_IsGreaterThan_CacheSize(KeyValuePair<string, object>[] data)
        {
            Assume.That(data.Length > CacheSize);

            foreach (var kv in data)
                _cache.Set(kv.Key, kv.Value);

            foreach (var kv in data.Skip(data.Length - CacheSize))
                Assert.That(_cache.Get(kv.Key), Is.EqualTo(kv.Value), "Then cache.size most recent items are stored");

            foreach (var kv in data.Take(data.Length - CacheSize))
                Assert.That(_cache.Get(kv.Key), Is.Null, "Then older items are discarded");
        }

        [Theory]
        public void When_Items_AreAccessed(KeyValuePair<string, object>[] data)
        {
            Assume.That(data.Length > CacheSize);

            foreach (var kv in data.Take(CacheSize))
                _cache.Set(kv.Key, kv.Value);

            var accessedItems = data.Take(CacheSize / 2).ToArray();
            foreach (var kv in accessedItems)
                _cache.Get(kv.Key);

            var lruItems = data.Skip(CacheSize / 2).Take(CacheSize / 2).ToArray();
            var newerItems = data.Skip(CacheSize).Take(CacheSize / 2).ToArray();
            foreach (var kv in newerItems)
                _cache.Set(kv.Key, kv.Value);

            foreach (var kv in accessedItems)
                Assert.That(_cache.Get(kv.Key), Is.EqualTo(kv.Value), "Then accessed items are kept in cache");

            foreach (var kv in newerItems)
                Assert.That(_cache.Get(kv.Key), Is.EqualTo(kv.Value), "Then newer items are kept in cache");

            foreach (var kv in lruItems)
                Assert.That(_cache.Get(kv.Key), Is.Null, "Then least recently used items are discarded");
        }

        [Theory]
        public void When_TheSameKey_IsInserted_MultipleTimes(KeyValuePair<string, object>[] data)
        {
            Assume.That(data.Length > CacheSize);

            var items = data.Take(CacheSize).ToArray();
            foreach (var kv in items)
                _cache.Set(kv.Key, kv.Value);

            var key = items.Last().Key;
            var value = items.Last().Value;
            for (var i = 0; i < 10; ++i)
                _cache.Set(key, value = Guid.NewGuid().ToString());

            foreach (var kv in items)
            {
                if (kv.Key == key)
                    Assert.That(_cache.Get(kv.Key), Is.EqualTo(value), "Then it stores the most recently set value");
                else
                    Assert.That(_cache.Get(kv.Key), Is.EqualTo(kv.Value), "Then it doesn't displace other items");
            }
        }

        private static IEnumerable<KeyValuePair<string, object>> GenerateData(int count)
        {
            for (var i = 0; i < count; i++)
                yield return new KeyValuePair<string, object>(i.ToString(), i);
        }
    }
}
