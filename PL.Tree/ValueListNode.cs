using System;
using System.Collections;
using System.Collections.Generic;

namespace PL.Tree
{
    /// <summary>
    /// Represents list with typed items.
    /// </summary>
    /// <typeparam name="T">Item type</typeparam>
    public class ValueListNode<T> : Node, IList<T>
    {
        private List<T> _items;

        /// <summary>
        /// Initializes a new instance of the ValueListNode class.
        /// </summary>
        public ValueListNode() => _items = new List<T>();

        /// <summary>
        /// Sets item values from source list item values.
        /// </summary>
        /// <param name="items">Source list</param>
        public void SetValue(IList<T> list)
        {
            bool valueChanged = false;
            lock (this)
            {
                if (_items.Count != list.Count)
                    valueChanged = true;
                else
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (!Equals(_items[i], list[i]))
                        {
                            valueChanged = true;
                            break;
                        }
                    }
                }
                _items = new List<T>(list);
            }
            if (valueChanged)
                OnValueChanged();
        }

        public T this[int index]
        {
            get
            {
                lock (this)
                    return _items[index];
            }
            set
            {
                lock (this)
                {
                    if (Equals(value, _items[index]))
                        return;
                    _items[index] = value;
                }
                OnValueChanged();
            }
        }

        public int Count
        {
            get
            {
                lock (this)
                    return _items.Count;
            }
        }

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            lock (this)
                _items.Add(item);
            OnValueChanged();
        }

        public void Clear()
        {
            if (Count == 0)
                return;
            lock (this)
                _items.Clear();
            OnValueChanged();
        }

        public bool Contains(T item)
        {
            lock (this)
                return _items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (this)
                _items.CopyTo(array, arrayIndex);
        }

        public int FindIndex(Predicate<T> match)
        {
            lock (this)
                return _items.FindIndex(match);
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (this)
                return new List<T>(_items).GetEnumerator();
        }

        public int IndexOf(T item)
        {
            lock (this)
                return _items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            lock (this)
                _items.Insert(index, item);
            OnValueChanged();
        }

        public bool Remove(T item)
        {
            bool removed;
            lock (this)
                removed = _items.Remove(item);
            if (removed)
                OnValueChanged();
            return removed;
        }

        public void RemoveAt(int index)
        {
            lock (this)
                _items.RemoveAt(index);
            OnValueChanged();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override object GetValue()
        {
            lock (this)
                return new List<T>(_items);
        }
        public override void SetValue (object value) => SetValue((IList<T>)value);
    }
}
