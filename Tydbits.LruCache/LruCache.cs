using System.Collections.Generic;

namespace Tydbits.LruCache
{
    public class LruCache : ICache
    {
        private readonly IDictionary<string, LinkedListNode<Entry>> _index;
        private readonly LinkedList<Entry> _inOrderOfUse;

        public LruCache(int size)
        {
            Size = size;
            _index = new Dictionary<string, LinkedListNode<Entry>>();
            _inOrderOfUse = new LinkedList<Entry>();
        }

        public int Size { get; }

        public void Set(string key, object value)
        {
            LinkedListNode<Entry> node;
            if (_index.TryGetValue(key, out node))
                UpdateExisting(node, value);
            else
                AddNew(key, value);
        }

        public object Get(string key)
        {
            LinkedListNode<Entry> node;
            if (!_index.TryGetValue(key, out node))
                return null;
            SetMostRecentlyUsed(node);
            return node.Value.Value;
        }

        private void SetMostRecentlyUsed(LinkedListNode<Entry> node)
        {
            _inOrderOfUse.Remove(node);
            _inOrderOfUse.AddLast(node);
        }

        private void AddNew(string key, object value)
        {
            while (_index.Count >= Size)
                Remove(_inOrderOfUse.First);
            _index[key] = _inOrderOfUse.AddLast(new Entry(key) {Value = value});
        }

        private void UpdateExisting(LinkedListNode<Entry> node, object value)
        {
            SetMostRecentlyUsed(node);
            node.Value.Value = value;
        }

        private void Remove(LinkedListNode<Entry> node)
        {
            _index.Remove(node.Value.Key);
            _inOrderOfUse.Remove(node);
        }

        private class Entry
        {
            public readonly string Key;
            public object Value;

            public Entry(string key)
            {
                Key = key;
            }
        }
    }
}
