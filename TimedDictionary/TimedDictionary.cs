using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace TimedDictionary
{
    public class TimedDictionary<T, TValue> : IDictionary<T, TValue> where T : notnull
    {
        public TValue this[T key] { get => _sourceDictionary[key].Value; set => _sourceDictionary[key].Value = value; }

        public ICollection<T> Keys => GetKeys();

        public ICollection<TValue> Values => GetValues();
        private readonly Dictionary<T, LinkedNode<T,TValue>> _sourceDictionary = new();
        
        private LinkedNode<T, TValue> _head = null;
        private LinkedNode<T, TValue> _tail = null;

        public int Count => _sourceDictionary.Count;

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(T key, TValue value)
        {
            var next = new LinkedNode<T,TValue>(key, value);
            _sourceDictionary.Add(key,next);
            if (_tail != null)
            {
                next.Previous = _tail;  
                _tail.Next = next;
            }
            else
            {
                _tail = next;
                _head = _tail;
            }
        }

        public void Add(KeyValuePair<T, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _sourceDictionary.Clear();
            _head = null;
            _tail = null;
        }

        public bool Contains(KeyValuePair<T, TValue> item)
        {
            return _sourceDictionary.TryGetValue(item.Key, out var node) && EqualityComparer<TValue>.Default.Equals(node.Value, item.Value);
        }

        public bool ContainsKey(T key)
        {
            return _sourceDictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<T, TValue>[] array, int arrayIndex)
        {
            _sourceDictionary.ToArray().CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<T, TValue>> GetEnumerator()
        {
            var node = _head;
            while (node != null)
            {
                yield return new KeyValuePair<T, TValue>(node.Key, node.Value);
                node = node.Next;
            }
        }

        private List<T> GetKeys()
        {
            var keys = new List<T>();
            var node = _head;
            while (node != null)
            {
                keys.Add(node.Key);
                node = node.Next;
            }
            return keys;
        }

        private List<TValue> GetValues()
        {
            var values = new List<TValue>();
            var node = _head;
            while (node != null)
            {
                values.Add(node.Value);
                node = node.Next;
            }
            return values;
        }

        public bool Remove(T key)
        {
            if(!_sourceDictionary.TryGetValue(key, out LinkedNode<T, TValue> node) || node == null)
            {
                return false;
            }
            var (next, previous) = (node.Next, node.Previous);
            next?.Previous = previous;
            previous?.Next = next;
            if (node == _head)
            {
                _head = next;
            }
            if (node == _tail)
            {
                _tail = previous;
            }
            _sourceDictionary.Remove(key);
            return true;
        }

        public bool Remove(KeyValuePair<T, TValue> item)
        {
            if (!_sourceDictionary.TryGetValue(item.Key, out LinkedNode<T, TValue> node) 
                || node == null 
                || !EqualityComparer<TValue>.Default.Equals(node.Value,item.Value))
            {
                return false;
            }
            var (next, previous) = (node.Next, node.Previous);
            next?.Previous = previous;
            previous?.Next = next;
            if (node == _head)
            {
                _head = next;
            }
            if (node == _tail)
            {
                _tail = previous;
            }
            _sourceDictionary.Remove(item.Key);
            return true;
        }

        public bool TryGetValue(T key, [MaybeNullWhen(false)] out TValue value)
        {
            if(!_sourceDictionary.TryGetValue(key, out var node))
            {
                value = default(TValue);
                return false;
            }
            value = node.Value;
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class LinkedNode<T, TValue>
        {
            public T Key { get; set; }
            public TValue Value { get; set; }
            public LinkedNode<T, TValue>? Next { get; set; }
            public LinkedNode<T, TValue>? Previous { get; set; }
            public LinkedNode(T key, TValue value)
            {
                Key = key;
                Value = value;
                Next = null;
                Previous = null;
            }
        }
    }
}
